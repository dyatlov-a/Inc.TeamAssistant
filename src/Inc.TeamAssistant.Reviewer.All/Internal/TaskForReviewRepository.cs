using Dapper;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Inc.TeamAssistant.Reviewer.All.Model;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed class TaskForReviewRepository : ITaskForReviewRepository
{
    private readonly string _connectionString;

    public TaskForReviewRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public async Task<IReadOnlyCollection<Guid>> Get(
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken cancellationToken)
    {
        if (states is null)
            throw new ArgumentNullException(nameof(states));

        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(@"
SELECT id AS id
FROM review.task_for_reviews
WHERE state = ANY(@states);",
            new { states = targetStates },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<Guid>(command);
        return results.ToArray();
    }

    public async Task<TaskForReview> GetById(Guid taskForReviewId, CancellationToken cancellationToken)
    {
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
JOIN review.players AS o ON o.id = t.owner_id
JOIN review.players AS r ON r.id = t.reviewer_id
WHERE t.id = @id;",
            new { id = taskForReviewId },
            flags: CommandFlags.Buffered,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<TaskForReview, Person, Person, TaskForReview>(
            command,
            (t, o, r) => t.Build(o, r),
            splitOn: "id");
        return results.Single();
    }

    public async Task Upsert(TaskForReview taskForReview, CancellationToken cancellationToken)
    {
        if (taskForReview is null)
            throw new ArgumentNullException(nameof(taskForReview));

        var command = new CommandDefinition(@"
INSERT INTO review.task_for_reviews (
    id,
    team_id,
    owner_id,
    reviewer_id,
    description,
    state,
    created,
    next_notification,
    accept_date,
    message_id,
    chat_id)
VALUES (
    @id,
    @team_id,
    @owner_id,
    @reviewer_id,
    @description,
    @state,
    @created,
    @next_notification,
    @accept_date,
    @message_id,
    @chat_id)
ON CONFLICT (id) DO UPDATE SET
    team_id = excluded.team_id,
    owner_id = excluded.owner_id,
    reviewer_id = excluded.reviewer_id,
    description = excluded.description,
    state = excluded.state,
    created = excluded.created,
    next_notification = excluded.next_notification,
    accept_date = excluded.accept_date,
    message_id = excluded.message_id,
    chat_id = excluded.chat_id;",
            new
            {
                id = taskForReview.Id,
                team_id = taskForReview.TeamId,
                description = taskForReview.Description,
                state = taskForReview.State,
                created = taskForReview.Created,
                next_notification = taskForReview.NextNotification,
                accept_date = taskForReview.AcceptDate,
                message_id = taskForReview.MessageId,
                chat_id = taskForReview.ChatId,
                owner_id = taskForReview.Owner.Id,
                reviewer_id = taskForReview.Reviewer.Id
            },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}