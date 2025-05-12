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
                ri.id AS id,
                ri.team_id AS teamid,
                ri.created AS created,
                ri.type AS type,
                ri.text AS text,
                ri.owner_id AS ownerid,
                ri.retro_session_id AS retrosessionid
            FROM retro.retro_items AS ri
            WHERE ri.team_id = @team_id;
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

    public async Task<RetroSession?> FindActive(Guid teamId, CancellationToken token)
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
            WHERE rs.team_id = @team_id;
            """,
            new
            {
                team_id = teamId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var result = await connection.QuerySingleOrDefaultAsync(command);
        
        return result;
    }
}