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
        var command = new CommandDefinition(@"
            SELECT
                p.id AS id,
                p.name AS name,
                p.username AS username,
                tm.team_id AS teamid,
                tm.leave_until AS leaveuntil
            FROM connector.teammates AS tm
            JOIN connector.persons AS p ON p.id = tm.person_id
            WHERE tm.team_id = @team_id;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var teammates = await connection
            .QueryAsync<(long Id, string Name, string? Username, Guid TeamId, DateTimeOffset? LeaveUntil)>(command);
        
        return teammates
            .Select(t => new TeammateDto(
                t.TeamId,
                t.Id,
                new Person(t.Id, t.Name, t.Username).DisplayName,
                t.LeaveUntil))
            .ToArray();
    }
}