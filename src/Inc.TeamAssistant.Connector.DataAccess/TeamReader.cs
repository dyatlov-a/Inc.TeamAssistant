using Dapper;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class TeamReader : ITeamReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public TeamReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<TeammateDto>> GetTeammates(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                tm.team_id AS teamid,
                p.id AS personid,
                p.name AS name,
                p.username AS username,
                tm.leave_until AS leaveuntil,
                tm.can_finalize AS canfinalize
            FROM connector.teammates AS tm
            JOIN connector.persons AS p ON p.id = tm.person_id
            WHERE tm.team_id = @team_id;
            """,
            new
            {
                team_id = teamId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var teammates = await connection.QueryAsync<TeammateDto>(command);
        
        return teammates.ToArray();
    }
    
    public async Task<IReadOnlyCollection<Person>> GetTeammates(
        Guid teamId,
        DateTimeOffset now,
        bool? canFinalize,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username
            FROM connector.persons AS p
            JOIN connector.teammates AS tm ON p.id = tm.person_id AND (tm.leave_until IS NULL OR tm.leave_until < @now)
            WHERE tm.team_id = @team_id AND (@can_finalize IS NULL OR tm.can_finalize = @can_finalize);
            """,
            new
            {
                team_id = teamId,
                can_finalize = canFinalize,
                now = now
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var persons = await connection.QueryAsync<Person>(command);
        return persons.ToArray();
    }
}