using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroReader : IRetroReader
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<RetroItem>> GetAll(Guid teamId, CancellationToken token)
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
            WHERE r.team_id = @team_id;
            """,
            new
            {
                team_id = teamId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var items = await connection.QueryAsync<RetroItem>(command);
        
        return items.ToArray();
    }
}