using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroSessionRepository : IRetroSessionRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroSessionRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<RetroSession?> Find(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                rs.id AS id,
                rs.team_id AS teamid,
                rs.created AS created,
                rs.state AS state,
                rs.facilitator_id AS facilitatorid
            FROM retro.retro_sessions AS rs
            WHERE id = @id;
            """,
            new
            {
                id = id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var retroSession = await connection.QuerySingleOrDefaultAsync<RetroSession>(command);
        
        return retroSession;
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

    public async Task Update(RetroSession retro, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(retro);
        
        var command = new CommandDefinition(
            """
            UPDATE retro.retro_sessions
            SET
                state = @state
            WHERE id = @id;
            """,
            new
            {
                id = retro.Id,
                state = retro.State
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}