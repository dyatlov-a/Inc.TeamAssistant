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

    public async Task<IReadOnlyCollection<Guid>> GetActive(CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(@"
SELECT id AS id
FROM review.task_for_reviews
WHERE is_active;",
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
    t.is_active AS isactive,
    t.next_notification AS nextnotification,
    o.id AS id,
    o.last_reviewer_id AS lastreviewerid,
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
INSERT INTO review.task_for_reviews (id, owner_id, reviewer_id, description, is_active, next_notification)
VALUES (@id, @owner_id, @reviewer_id, @description, @is_active, @next_notification)
ON CONFLICT (id) DO UPDATE SET
owner_id = excluded.owner_id,
reviewer_id = excluded.reviewer_id,
description = excluded.description,
is_active = excluded.is_active,
next_notification = excluded.next_notification;

UPDATE review.players
SET last_reviewer_id = @last_reviewer_id
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
                is_active = taskForReview.IsActive,
                next_notification = taskForReview.NextNotification,

                owner_last_reviewer_id = taskForReview.Owner.LastReviewerId,

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