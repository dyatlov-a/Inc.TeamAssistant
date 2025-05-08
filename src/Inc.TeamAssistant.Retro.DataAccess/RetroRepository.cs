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

    public async Task<RetroItem?> FindItem(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                r.id AS id,
                r.team_id AS teamid,
                r.created AS created,
                r.type AS type,
                r.text AS text,
                r.owner_id AS ownerid
            FROM retro.retro_items AS r
            WHERE r.id = @id;
            """,
            new
            {
                id = id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var item = await connection.QuerySingleOrDefaultAsync<RetroItem>(command);
        
        return item;
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
                owner_id)
            VALUES (
                @id,
                @team_id,
                @created,
                @type,
                @text,
                @owner_id)
            ON CONFLICT (id) DO UPDATE SET
                team_id = EXCLUDED.team_id,
                created = EXCLUDED.created,
                type = EXCLUDED.type,
                text = EXCLUDED.text,
                owner_id = EXCLUDED.owner_id;
            """,
            new
            {
                id = item.Id,
                team_id = item.TeamId,
                created = item.Created,
                type = item.Type,
                text = item.Text,
                owner_id = item.OwnerId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}