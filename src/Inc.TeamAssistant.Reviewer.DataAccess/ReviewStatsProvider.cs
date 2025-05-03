using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Primitives.Features.PersonStats;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class ReviewStatsProvider : IPersonStatsProvider
{
    private readonly IConnectionFactory _connectionFactory;

    public ReviewStatsProvider(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public string FeatureName => "Review";
    
    public async Task<IReadOnlyDictionary<long, int>> GetStats(
        IReadOnlyCollection<long> personIds,
        DateTimeOffset from,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(personIds);
        
        ArgumentNullException.ThrowIfNull(personIds);

        var state = (int)TaskForReviewState.Accept;
        var command = new CommandDefinition(
            """
            SELECT
                t.reviewer_id AS personid,
                count(*) AS eventscount
            FROM review.task_for_reviews AS t
            WHERE t.created > @from AND t.reviewer_id = ANY(@person_ids) AND t.state = @state
            GROUP BY t.reviewer_id;
            """,
            new
            {
                person_ids = personIds.ToArray(),
                from = from.UtcDateTime,
                state = state
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<(long PersonId, int EventsCount)>(command);

        return results.ToDictionary();
    }
}