using Dapper;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class ReviewAnalyticsReader : IReviewAnalyticsReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public ReviewAnalyticsReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }


    public async Task<IReadOnlyCollection<HistoryByTeamItemDto>> GetReviewHistory(
        Guid teamId,
        DateTimeOffset from,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT p.id AS personid, p.name AS name, p.username AS username, COUNT(*) AS count
            FROM review.task_for_reviews AS t
            JOIN connector.persons AS p ON p.id = t.reviewer_id
            WHERE team_id = @team_id
            GROUP BY p.id, p.name, p.username;",
            new
            {
                team_id = teamId,
                from
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var items = await connection.QueryAsync<(long PersonId, string Name, string Username, int Count)>(command);
        var results = items
            .Select(i => new HistoryByTeamItemDto(new Person(i.PersonId, i.Name, i.Username).DisplayName, i.Count))
            .ToArray();

        return results;
    }

    public async Task<IReadOnlyCollection<HistoryByTeamItemDto>> GetRequestsHistory(
        Guid teamId,
        DateTimeOffset from,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT p.id AS personid, p.name AS name, p.username AS username, COUNT(*) AS count
            FROM review.task_for_reviews AS t
            JOIN connector.persons AS p ON p.id = t.owner_id
            WHERE team_id = @team_id
            GROUP BY p.id, p.name, p.username;",
            new
            {
                team_id = teamId,
                from
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var items = await connection.QueryAsync<(long PersonId, string Name, string Username, int Count)>(command);
        var results = items
            .Select(i => new HistoryByTeamItemDto(new Person(i.PersonId, i.Name, i.Username).DisplayName, i.Count))
            .ToArray();

        return results;
    }
}