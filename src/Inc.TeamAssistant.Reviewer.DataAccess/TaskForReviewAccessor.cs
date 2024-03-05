using Dapper;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class TaskForReviewAccessor : ITaskForReviewAccessor
{
    private readonly string _connectionString;

    public TaskForReviewAccessor(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public async Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        IReadOnlyCollection<TaskForReviewState> states,
        int limit,
        CancellationToken token)
    {
        if (states is null)
            throw new ArgumentNullException(nameof(states));

        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(@"
SELECT
    t.id AS id,
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
    t.chat_id AS chatid
FROM review.task_for_reviews AS t
WHERE t.state = ANY(@states) AND t.next_notification < @now
ORDER BY t.next_notification
LIMIT @limit;",
            new { now, states = targetStates, limit },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<TaskForReview>(command);

        return results.ToArray();
    }

    public async Task Update(IReadOnlyCollection<TaskForReview> taskForReviews, CancellationToken token)
    {
        if (taskForReviews is null)
            throw new ArgumentNullException(nameof(taskForReviews));

        var ids = new List<Guid>(taskForReviews.Count);
        var nextNotifications = new List<DateTimeOffset>(taskForReviews.Count);

        foreach (var taskForReview in taskForReviews)
        {
            ids.Add(taskForReview.Id);
            nextNotifications.Add(taskForReview.NextNotification);
        }

        var command = new CommandDefinition(@"
UPDATE review.task_for_reviews AS et
SET next_notification = t.next_notification
FROM UNNEST(@ids, @next_notifications) AS t(id, next_notification)
WHERE et.id = t.id;",
            new
            {
                ids,
                next_notifications = nextNotifications
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}