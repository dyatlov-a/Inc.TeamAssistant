using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class TaskForReviewReader : ITaskForReviewReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public TaskForReviewReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        IReadOnlyCollection<TaskForReviewState> states,
        int limit,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);

        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(@"
SELECT
    t.id AS id,
    bot_id AS botid,
    t.team_id AS teamid,
    t.strategy AS strategy,
    t.owner_id AS ownerid,
    t.reviewer_id AS reviewerid,
    t.description AS description,
    t.state AS state,
    t.created AS created,
    t.next_notification AS nextnotification,
    t.accept_date AS acceptdate,
    t.message_id AS messageid,
    t.chat_id AS chatid,
    t.original_reviewer_id AS originalreviewerid
FROM review.task_for_reviews AS t
WHERE t.state = ANY(@states) AND t.next_notification < @now
ORDER BY t.next_notification
LIMIT @limit;",
            new { now, states = targetStates, limit },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<TaskForReview>(command);

        return results.ToArray();
    }

    public async Task<IReadOnlyCollection<TaskForReview>> GetTasksByPerson(
        long personId,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);

        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(@"
SELECT
    t.id AS id,
    bot_id AS botid,
    t.team_id AS teamid,
    t.strategy AS strategy,
    t.owner_id AS ownerid,
    t.reviewer_id AS reviewerid,
    t.description AS description,
    t.state AS state,
    t.created AS created,
    t.next_notification AS nextnotification,
    t.accept_date AS acceptdate,
    t.message_id AS messageid,
    t.chat_id AS chatid,
    t.original_reviewer_id AS originalreviewerid
FROM review.task_for_reviews AS t
WHERE t.reviewer_id = @person_id AND t.state = ANY(@states);",
            new
            {
                person_id = personId,
                states = targetStates
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<TaskForReview>(command);

        return results.ToArray();
    }

    public async Task<bool> HasReassignFromDate(long personId, DateTimeOffset date, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT true
            FROM review.task_for_reviews AS t
            WHERE t.original_reviewer_id = @person_id AND t.created >= @date
            ORDER BY t.created DESC
            OFFSET 0
            LIMIT 1;",
            new
            {
                person_id = personId,
                date = date.UtcDateTime
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        return await connection.QuerySingleOrDefaultAsync<bool>(command);
    }
}