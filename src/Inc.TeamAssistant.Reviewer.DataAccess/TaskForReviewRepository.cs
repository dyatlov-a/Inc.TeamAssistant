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

    public async Task<IReadOnlyCollection<TaskForReview>> Get(
        Guid teamId,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);
        
        var targetStates = states.Select(s => (int)s).ToArray();
        var command = new CommandDefinition(@"
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
                t.review_intervals AS reviewintervals
            FROM review.task_for_reviews AS t
            WHERE t.team_id = @team_id AND t.state = ANY(@states);",
            new
            {
                team_id = teamId,
                states = targetStates,
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var results = await connection.QueryAsync<TaskForReview>(command);

        return results.ToArray();
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
                t.review_intervals AS reviewintervals
            FROM review.task_for_reviews AS t
            WHERE t.id = @id;",
            new { id = taskForReviewId },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        return await connection.QuerySingleAsync<TaskForReview>(command);
    }

    public async Task Upsert(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var reviewIntervals = JsonSerializer.Serialize(taskForReview.ReviewIntervals);

        var command = new CommandDefinition(@"
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
                review_intervals)
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
                @review_intervals::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                bot_id = excluded.bot_id,
                team_id = excluded.team_id,
                strategy = excluded.strategy,
                owner_id = excluded.owner_id,
                owner_message_id = excluded.owner_message_id,
                reviewer_id = excluded.reviewer_id,
                reviewer_message_id = excluded.reviewer_message_id,
                description = excluded.description,
                state = excluded.state,
                created = excluded.created,
                next_notification = excluded.next_notification,
                accept_date = excluded.accept_date,
                message_id = excluded.message_id,
                chat_id = excluded.chat_id,
                original_reviewer_id = excluded.original_reviewer_id,
                review_intervals = excluded.review_intervals;",
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
                owner_message_id = taskForReview.OwnerMessageId,
                reviewer_id = taskForReview.ReviewerId,
                reviewer_message_id = taskForReview.ReviewerMessageId,
                original_reviewer_id = taskForReview.OriginalReviewerId,
                review_intervals = reviewIntervals
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }

    public async Task<long?> FindLastReviewer(Guid teamId, long ownerId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                t.reviewer_id AS reviewerid
            FROM review.task_for_reviews AS t
            WHERE t.team_id = @team_id AND t.owner_id = @owner_id AND t.strategy != @next_reviewer_type__target
            ORDER BY t.created DESC
            OFFSET 0
            LIMIT 1;",
            new
            {
                team_id = teamId,
                owner_id = ownerId,
                next_reviewer_type__target = (int)NextReviewerType.Target
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        return await connection.QuerySingleOrDefaultAsync<long?>(command);
    }
}