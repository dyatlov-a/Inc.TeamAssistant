using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class TaskForReviewRepository : ITaskForReviewRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public TaskForReviewRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<TaskForReview?> Find(Guid taskForReviewId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.bot_id AS botid,
                t.team_id AS teamid,
                t.strategy AS strategy,
                t.owner_id AS ownerid,
                t.owner_message_id AS ownermessageid,
                t.reviewer_id AS reviewerid,
                t.reviewer_message_id AS reviewermessageid,
                t.description AS description,
                t.state AS state,
                t.created AS created,
                t.next_notification AS nextnotification,
                t.accept_date AS acceptdate,
                t.message_id AS messageid,
                t.chat_id AS chatid,
                t.original_reviewer_id AS originalreviewerid,
                t.original_reviewer_message_id AS originalreviewermessageid,
                t.first_reviewer_id AS firstreviewerid,
                t.first_reviewer_message_id AS firstreviewermessageid,
                t.review_intervals AS reviewintervals,
                t.comments AS comments
            FROM review.task_for_reviews AS t
            WHERE t.id = @id;
            """,
            new
            {
                id = taskForReviewId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<TaskForReview>(command);
    }

    public async Task Upsert(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var reviewIntervals = JsonSerializer.Serialize(taskForReview.ReviewIntervals);
        var comments = JsonSerializer.Serialize(taskForReview.Comments);

        var command = new CommandDefinition(
            """
            INSERT INTO review.task_for_reviews (
                id,
                bot_id,
                team_id,
                strategy,
                owner_id,
                owner_message_id,
                reviewer_id,
                reviewer_message_id,
                description,
                state,
                created,
                next_notification,
                accept_date,
                message_id,
                chat_id,
                original_reviewer_id,
                original_reviewer_message_id,
                first_reviewer_id,
                first_reviewer_message_id,
                review_intervals,
                comments)
            VALUES (
                @id,
                @bot_id,
                @team_id,
                @strategy,
                @owner_id,
                @owner_message_id,
                @reviewer_id,
                @reviewer_message_id,
                @description,
                @state,
                @created,
                @next_notification,
                @accept_date,
                @message_id,
                @chat_id,
                @original_reviewer_id,
                @original_reviewer_message_id,
                @first_reviewer_id,
                @first_reviewer_message_id,
                @review_intervals::jsonb,
                @comments::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                bot_id = EXCLUDED.bot_id,
                team_id = EXCLUDED.team_id,
                strategy = EXCLUDED.strategy,
                owner_id = EXCLUDED.owner_id,
                owner_message_id = EXCLUDED.owner_message_id,
                reviewer_id = EXCLUDED.reviewer_id,
                reviewer_message_id = EXCLUDED.reviewer_message_id,
                description = EXCLUDED.description,
                state = EXCLUDED.state,
                created = EXCLUDED.created,
                next_notification = EXCLUDED.next_notification,
                accept_date = EXCLUDED.accept_date,
                message_id = EXCLUDED.message_id,
                chat_id = EXCLUDED.chat_id,
                original_reviewer_id = EXCLUDED.original_reviewer_id,
                original_reviewer_message_id = EXCLUDED.original_reviewer_message_id,
                first_reviewer_id = EXCLUDED.first_reviewer_id,
                first_reviewer_message_id = EXCLUDED.first_reviewer_message_id,
                review_intervals = EXCLUDED.review_intervals,
                comments = EXCLUDED.comments;
            """,
            new
            {
                id = taskForReview.Id,
                bot_id = taskForReview.BotId,
                team_id = taskForReview.TeamId,
                strategy = (int)taskForReview.Strategy,
                description = taskForReview.Description,
                state = (int)taskForReview.State,
                created = taskForReview.Created,
                next_notification = taskForReview.NextNotification,
                accept_date = taskForReview.AcceptDate,
                message_id = taskForReview.MessageId,
                chat_id = taskForReview.ChatId,
                owner_id = taskForReview.OwnerId,
                owner_message_id = taskForReview.OwnerMessageId,
                reviewer_id = taskForReview.ReviewerId,
                reviewer_message_id = taskForReview.ReviewerMessageId,
                original_reviewer_id = taskForReview.OriginalReviewerId,
                original_reviewer_message_id = taskForReview.OriginalReviewerMessageId,
                first_reviewer_id = taskForReview.FirstReviewerId,
                first_reviewer_message_id = taskForReview.FirstReviewerMessageId,
                review_intervals = reviewIntervals,
                comments = comments
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}