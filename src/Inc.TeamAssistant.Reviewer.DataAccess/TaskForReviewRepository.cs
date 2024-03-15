using Dapper;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class TaskForReviewRepository : ITaskForReviewRepository
{
    private readonly string _connectionString;

    public TaskForReviewRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public async Task<TaskForReview> GetById(Guid taskForReviewId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                t.id AS id,
                t.bot_id AS botid,
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
                t.has_concrete_reviewer AS hasconcretereviewer
            FROM review.task_for_reviews AS t
            WHERE t.id = @id;",
            new { id = taskForReviewId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);
        
        return await connection.QuerySingleAsync<TaskForReview>(command);
    }

    public async Task Upsert(TaskForReview taskForReview, CancellationToken token)
    {
        if (taskForReview is null)
            throw new ArgumentNullException(nameof(taskForReview));

        var command = new CommandDefinition(@"
            INSERT INTO review.task_for_reviews (
                id,
                bot_id,
                team_id,
                strategy,
                owner_id,
                reviewer_id,
                description,
                state,
                created,
                next_notification,
                accept_date,
                message_id,
                chat_id,
                has_concrete_reviewer)
            VALUES (
                @id,
                @bot_id,
                @team_id,
                @strategy,
                @owner_id,
                @reviewer_id,
                @description,
                @state,
                @created,
                @next_notification,
                @accept_date,
                @message_id,
                @chat_id,
                @has_concrete_reviewer)
            ON CONFLICT (id) DO UPDATE SET
                bot_id = excluded.bot_id,
                team_id = excluded.team_id,
                strategy = excluded.strategy,
                owner_id = excluded.owner_id,
                reviewer_id = excluded.reviewer_id,
                description = excluded.description,
                state = excluded.state,
                created = excluded.created,
                next_notification = excluded.next_notification,
                accept_date = excluded.accept_date,
                message_id = excluded.message_id,
                chat_id = excluded.chat_id,
                has_concrete_reviewer = excluded.has_concrete_reviewer;",
            new
            {
                id = taskForReview.Id,
                bot_id = taskForReview.BotId,
                team_id = taskForReview.TeamId,
                strategy = taskForReview.Strategy,
                description = taskForReview.Description,
                state = taskForReview.State,
                created = taskForReview.Created,
                next_notification = taskForReview.NextNotification,
                accept_date = taskForReview.AcceptDate,
                message_id = taskForReview.MessageId,
                chat_id = taskForReview.ChatId,
                owner_id = taskForReview.OwnerId,
                reviewer_id = taskForReview.ReviewerId,
                has_concrete_reviewer = taskForReview.HasConcreteReviewer
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }

    public async Task RetargetAndLeave(
        Guid teamId,
        long fromId,
        long toId,
        DateTimeOffset nextNotification,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            UPDATE review.task_for_reviews
            SET
                reviewer_id = @to_person_id,
                next_notification = @next_notification
            WHERE reviewer_id = @from_person_id AND team_id = @team_id AND state != @is_archived;

            DELETE FROM connector.teammates
            WHERE person_id = @from_person_id AND team_id = @team_id;",
            new
            {
                team_id = teamId,
                from_person_id = fromId,
                to_person_id = toId,
                next_notification = nextNotification,
                is_archived = (int)TaskForReviewState.IsArchived
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }

    public async Task<long?> FindLastReviewer(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                t.reviewer_id AS reviewerid
            FROM review.task_for_reviews AS t
            WHERE t.team_id = @team_id AND NOT t.has_concrete_reviewer
            ORDER BY t.created DESC
            OFFSET 0
            LIMIT 1;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = new NpgsqlConnection(_connectionString);
        
        return await connection.QuerySingleOrDefaultAsync<long?>(command);
    }
}