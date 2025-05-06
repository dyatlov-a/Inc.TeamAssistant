using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroCardPoolRepository : IRetroCardPoolRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroCardPoolRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<RetroCardPool?> Find(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                rcp.id AS id,
                rcp.name AS name,
                rcp.owner_id AS ownerid
            FROM retro.retro_card_pools AS rcp
            WHERE rcp.id = @id;
            """,
            new
            {
                id = id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<RetroCardPool>(command);
    }

    public async Task Upsert(RetroCardPool pool, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(pool);
        
        var command = new CommandDefinition(
            """
            INSERT INTO retro.retro_card_pools (
                id,
                name,
                owner_id)
            VALUES (
                @id,
                @name,
                @owner_id)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                owner_id = EXCLUDED.owner_id;
            """,
            new
            {
                id = pool.Id,
                name = pool.Name,
                owner_id = pool.OwnerId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}