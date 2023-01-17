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
    t.owner_id AS ownerid,
    t.reviewer_id AS reviewerid,
    t.description AS description,
    t.state AS state,
    t.created AS created,
    t.next_notification AS nextnotification,
    t.accept_date AS acceptdate,
    t.message_id AS messageid,
    t.chat_id AS chatid,
    o.id AS id,
    o.team_id AS teamid,
    o.person__id AS personid,
    o.person__language_id AS personlanguageid,
    o.person__first_name AS personfirstname,
    o.person__last_name AS personlastname,
    o.person__username AS personusername,
    r.id AS id,
    r.team_id AS teamid,
    r.person__id AS personid,
    r.person__language_id AS personlanguageid,
    r.person__first_name AS personfirstname,
    r.person__last_name AS personlastname,
    r.person__username AS personusername
FROM review.task_for_reviews AS t
JOIN review.players AS o ON o.id = t.owner_id
JOIN review.players AS r ON r.id = t.reviewer_id
WHERE t.id = @id;",
            new { id = taskForReviewId },
            flags: CommandFlags.Buffered,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<TaskForReview, DbPlayer, DbPlayer, TaskForReview>(
            command,
            (t, o, r) => t.Build(
                Player.Build(
                    o.Id,
                    o.TeamId,
                    new Person(o.PersonId, o.PersonLanguageId, o.PersonFirstName, o.PersonLastName, o.PersonUsername)),
                Player.Build(
                    r.Id,
                    r.TeamId,
                    new Person(r.PersonId, r.PersonLanguageId, r.PersonFirstName, r.PersonLastName, r.PersonUsername))),
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
    chat_id = excluded.chat_id;

UPDATE review.players
SET
    team_id = @owner_person__team_id,
    person__id = @owner_person__id,
    person__language_id = @owner_person__language_id,
    person__first_name = @owner_person__first_name,
    person__last_name = @owner_person__last_name,
    person__username = @owner_person__username
WHERE id = @owner_id;

UPDATE review.players
SET
    team_id = @reviewer_person__team_id,
    person__id = @reviewer_person__id,
    person__language_id = @reviewer_person__language_id,
    person__first_name = @reviewer_person__first_name,
    person__last_name = @reviewer_person__last_name,
    person__username = @reviewer_person__username
WHERE id = @reviewer_id;",
            new
            {
                id = taskForReview.Id,
                team_id = taskForReview.TeamId,
                owner_id = taskForReview.OwnerId,
                reviewer_id = taskForReview.ReviewerId,
                description = taskForReview.Description,
                state = taskForReview.State,
                created = taskForReview.Created,
                next_notification = taskForReview.NextNotification,
                accept_date = taskForReview.AcceptDate,
                message_id = taskForReview.MessageId,
                chat_id = taskForReview.ChatId,
                
                owner_person__team_id = taskForReview.Owner.TeamId,
                owner_person__id = taskForReview.Owner.Person.Id,
                owner_person__language_id = taskForReview.Owner.Person.LanguageId,
                owner_person__first_name = taskForReview.Owner.Person.FirstName,
                owner_person__last_name = taskForReview.Owner.Person.LastName,
                owner_person__username = taskForReview.Owner.Person.Username,
                
                reviewer_person__team_id = taskForReview.Reviewer.TeamId,
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