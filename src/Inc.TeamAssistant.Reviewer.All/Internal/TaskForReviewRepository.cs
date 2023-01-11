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
    t.owner_id AS ownerid,
    t.reviewer_id AS reviewerid,
    t.description AS description,
    t.state AS state,
    t.next_notification AS nextnotification,
    t.accept_date AS acceptdate,
    o.id AS id,
    o.last_reviewer_id AS lastreviewerid,
    o.language_id AS languageid,
    o.user_id AS userid,
    r.id AS id,
    r.user_id AS userid,
    r.name AS name,
    r.language_id AS languageid,
    r.login AS login
FROM review.task_for_reviews AS t
JOIN review.players AS o ON o.id = t.owner_id
JOIN review.players AS r ON r.id = t.reviewer_id
WHERE t.id = @id;",
            new { id = taskForReviewId },
            flags: CommandFlags.Buffered,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<TaskForReview, PlayerAsOwner, PlayerAsReviewer, TaskForReview>(
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
INSERT INTO review.task_for_reviews (id, owner_id, reviewer_id, description, state, next_notification, accept_date)
VALUES (@id, @owner_id, @reviewer_id, @description, @state, @next_notification, @accept_date)
ON CONFLICT (id) DO UPDATE SET
owner_id = excluded.owner_id,
reviewer_id = excluded.reviewer_id,
description = excluded.description,
state = excluded.state,
next_notification = excluded.next_notification,
accept_date = excluded.accept_date;

UPDATE review.players
SET
    last_reviewer_id = @owner_last_reviewer_id,
    user_id = @owner_user_id,
    language_id = @owner_language_id
WHERE id = @owner_id;

UPDATE review.players
SET
    user_id = @reviewer_user_id,
    language_id = @reviewer_language_id,
    name = @reviewer_name,
    login = @reviewer_login
WHERE id = @reviewer_id;",
            new
            {
                id = taskForReview.Id,
                owner_id = taskForReview.OwnerId,
                reviewer_id = taskForReview.ReviewerId,
                description = taskForReview.Description,
                state = taskForReview.State,
                next_notification = taskForReview.NextNotification,
                accept_date = taskForReview.AcceptDate,

                owner_last_reviewer_id = taskForReview.Owner.LastReviewerId,
                owner_user_id = taskForReview.Owner.UserId,
                owner_language_id = taskForReview.Owner.LanguageId,

                reviewer_user_id = taskForReview.Reviewer.UserId,
                reviewer_language_id = taskForReview.Reviewer.LanguageId,
                reviewer_name = taskForReview.Reviewer.Name,
                reviewer_login = taskForReview.Reviewer.Login
            },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}