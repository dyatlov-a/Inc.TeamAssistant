using Dapper;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Model;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

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
        CancellationToken cancellationToken)
    {
        if (states is null)
            throw new ArgumentNullException(nameof(states));

        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(@"
SELECT
    t.id AS id,
    t.team_id AS teamid,
    t.description AS description,
    t.state AS state,
    t.created AS created,
    t.next_notification AS nextnotification,
    t.accept_date AS acceptdate,
    t.message_id AS messageid,
    t.chat_id AS chatid,
    o.id AS id,
    o.language_id AS languageid,
    o.first_name AS firstname,
    o.last_name AS lastname,
    o.username AS username,
    r.id AS id,
    r.language_id AS languageid,
    r.first_name AS firstname,
    r.last_name AS lastname,
    r.username AS username
FROM review.task_for_reviews AS t
JOIN review.persons AS o ON o.id = t.owner_id
JOIN review.persons AS r ON r.id = t.reviewer_id
WHERE t.state = ANY(@states) AND t.next_notification < @now
ORDER BY t.next_notification
LIMIT @limit;",
            new { now, states = targetStates, limit },
            flags: CommandFlags.Buffered,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<TaskForReview, Person, Person, TaskForReview>(
            command,
            (t, o, r) => t.Build(o, r),
            splitOn: "id");

        return results.ToArray();
    }

    public async Task Update(IReadOnlyCollection<TaskForReview> taskForReviews, CancellationToken cancellationToken)
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
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}