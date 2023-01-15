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
    t.message_id AS messageid,
    t.chat_id AS chatid,
    o.id AS id,
    o.last_reviewer_id AS lastreviewerid,
    o.person__id AS id,
    o.person__language_id AS languageid,
    o.person__first_name AS firstname,
    o.person__last_name AS lastname,
    o.person__username AS username,
    r.id AS id,
    r.person__id AS id,
    r.person__language_id AS languageid,
    r.person__first_name AS firstname,
    r.person__last_name AS lastname,
    r.person__username AS username
FROM review.task_for_reviews AS t
JOIN review.players AS o ON o.id = t.owner_id
JOIN review.players AS r ON r.id = t.reviewer_id
WHERE t.id = @id;",
            new { id = taskForReviewId },
            flags: CommandFlags.Buffered,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<TaskForReview, PlayerAsOwner, Person, PlayerAsReviewer, Person, TaskForReview>(
            command,
            (t, o, op, r, rp) => t.Build(o.Build(op), r.Build(rp)),
            splitOn: "id");
        return results.Single();
    }

    public async Task Upsert(TaskForReview taskForReview, CancellationToken cancellationToken)
    {
        if (taskForReview is null)
            throw new ArgumentNullException(nameof(taskForReview));

        var command = new CommandDefinition(@"
INSERT INTO review.task_for_reviews (id, owner_id, reviewer_id, description, state, next_notification, accept_date, message_id, chat_id)
VALUES (@id, @owner_id, @reviewer_id, @description, @state, @next_notification, @accept_date, @message_id, @chat_id)
ON CONFLICT (id) DO UPDATE SET
owner_id = excluded.owner_id,
reviewer_id = excluded.reviewer_id,
description = excluded.description,
state = excluded.state,
next_notification = excluded.next_notification,
accept_date = excluded.accept_date,
message_id = excluded.message_id,
chat_id = excluded.chat_id;

UPDATE review.players
SET
    last_reviewer_id = @owner__last_reviewer_id,
    person__id = @owner_person__id,
    person__language_id = @owner_person__language_id,
    person__first_name = @owner_person__first_name,
    person__last_name = @owner_person__last_name,
    person__username = @owner_person__username
WHERE id = @owner_id;

UPDATE review.players
SET
    person__id = @reviewer_person__id,
    person__language_id = @reviewer_person__language_id,
    person__first_name = @reviewer_person__first_name,
    person__last_name = @reviewer_person__last_name,
    person__username = @reviewer_person__username
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
                message_id = taskForReview.MessageId,
                chat_id = taskForReview.ChatId,

                owner__last_reviewer_id = taskForReview.Owner.LastReviewerId,
                owner_person__id = taskForReview.Owner.Person.Id,
                owner_person__language_id = taskForReview.Owner.Person.LanguageId,
                owner_person__first_name = taskForReview.Owner.Person.FirstName,
                owner_person__last_name = taskForReview.Owner.Person.LastName,
                owner_person__username = taskForReview.Owner.Person.Username,
                
                reviewer_person__id = taskForReview.Reviewer.Person.Id,
                reviewer_person__language_id = taskForReview.Reviewer.Person.LanguageId,
                reviewer_person__first_name = taskForReview.Reviewer.Person.FirstName,
                reviewer_person__last_name = taskForReview.Reviewer.Person.LastName,
                reviewer_person__username = taskForReview.Reviewer.Person.Username
            },
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}