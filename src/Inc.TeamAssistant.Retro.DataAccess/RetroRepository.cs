using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroRepository : IRetroRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<RetroItem?> Find(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                ri.id AS id,
                ri.team_id AS teamid,
                ri.created AS created,
                ri.type AS type,
                ri.text AS text,
                ri.owner_id AS ownerid,
                ri.retro_session_id AS retrosessionid,
                rs.id AS id,
                rs.team_id AS teamid,
                rs.created AS created,
                rs.state AS state,
                rs.facilitator_id AS facilitatorid
            FROM retro.retro_items AS ri
            LEFT JOIN retro.retro_sessions AS rs ON ri.retro_session_id = rs.id
            WHERE ri.id = @item_id;
            """,
            new
            {
                item_id = id
            },
            flags: CommandFlags.Buffered,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var query = await connection.QueryAsync<RetroItem, RetroSession, RetroItem>(
            command,
            (ri, rs) => ri.MapRetroSession(rs),
            "id");
        
        return query.SingleOrDefault();
    }

    public async Task Upsert(RetroItem item, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var command = new CommandDefinition(
            """
            INSERT INTO retro.retro_items (
                id,
                team_id,
                created,
                type,
                text,
                owner_id,
                retro_session_id)
            VALUES (
                @id,
                @team_id,
                @created,
                @type,
                @text,
                @owner_id,
                @retro_session_id)
            ON CONFLICT (id) DO UPDATE SET
                team_id = EXCLUDED.team_id,
                created = EXCLUDED.created,
                type = EXCLUDED.type,
                text = EXCLUDED.text,
                owner_id = EXCLUDED.owner_id,
                retro_session_id = EXCLUDED.retro_session_id;
            """,
            new
            {
                id = item.Id,
                team_id = item.TeamId,
                created = item.Created,
                type = item.Type,
                text = item.Text,
                owner_id = item.OwnerId,
                retro_session_id = item.RetroSessionId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }

    public async Task Create(RetroSession retro, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(retro);
        
        var upsertRetroCommand = new CommandDefinition(
            """
            INSERT INTO retro.retro_sessions (
                id,
                team_id,
                created,
                state,
                facilitator_id)
            VALUES (
                @id,
                @team_id,
                @created,
                @state,
                @facilitator_id)
            ON CONFLICT (id) DO UPDATE SET
                team_id = EXCLUDED.team_id,
                created = EXCLUDED.created,
                state = EXCLUDED.state,
                facilitator_id = EXCLUDED.facilitator_id;
            """,
            new
            {
                id = retro.Id,
                team_id = retro.TeamId,
                created = retro.Created,
                state = retro.State,
                facilitator_id = retro.FacilitatorId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        var attachItemsCommand = new CommandDefinition(
            """
            UPDATE retro.retro_items AS ri
            SET 
                retro_session_id = @retro_session_id
            WHERE ri.team_id = @team_id AND ri.retro_session_id IS NULL;
            """,
            new
            {
                retro_session_id = retro.Id,
                team_id = retro.TeamId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertRetroCommand);
        await connection.ExecuteAsync(attachItemsCommand);

        await transaction.CommitAsync(token);
    }

    public async Task Remove(RetroItem item, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var command = new CommandDefinition(
            """
            DELETE FROM retro.retro_items AS ri
            WHERE ri.id = @item_id;
            """,
            new
            {
                item_id = item.Id
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}