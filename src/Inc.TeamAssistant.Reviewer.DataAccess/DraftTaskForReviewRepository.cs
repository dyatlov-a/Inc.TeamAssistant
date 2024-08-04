using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.DataAccess;

internal sealed class DraftTaskForReviewRepository : IDraftTaskForReviewRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public DraftTaskForReviewRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<DraftTaskForReview?> Find(long chatId, int messageId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT 
                d.id AS id,
                d.team_id AS teamid,
                d.strategy AS strategy,
                d.chat_id AS chatid,
                d.message_id AS messageid,
                d.description AS description,
                d.target_person_id AS targetpersonid,
                d.preview_message_id AS previewmessageid,
                d.created AS created
            FROM review.draft_task_for_reviews AS d
            WHERE d.chat_id = @chat_id AND d.message_id = @message_id;",
            new
            {
                chat_id = chatId,
                message_id = messageId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleAsync<DraftTaskForReview>(command);
    }

    public async Task<DraftTaskForReview> GetById(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT 
                d.id AS id,
                d.team_id AS teamid,
                d.strategy AS strategy,
                d.chat_id AS chatid,
                d.message_id AS messageid,
                d.description AS description,
                d.target_person_id AS targetpersonid,
                d.preview_message_id AS previewmessageid,
                d.created AS created
            FROM review.draft_task_for_reviews AS d
            WHERE d.id = @id;",
            new { id },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        return await connection.QuerySingleAsync<DraftTaskForReview>(command);
    }

    public async Task Upsert(DraftTaskForReview draft, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(nameof(draft));
        
        var command = new CommandDefinition(@"
            INSERT INTO review.draft_task_for_reviews (
                id,
                team_id,
                strategy,
                chat_id,
                message_id,
                description,
                target_person_id,
                preview_message_id,
                created)
            VALUES (
                @id,
                @team_id,
                @strategy,
                @chat_id,
                @message_id,
                @description,
                @target_person_id,
                @preview_message_id,
                @created)
            ON CONFLICT (id) DO UPDATE SET
                team_id = excluded.team_id,
                strategy = excluded.strategy,
                chat_id = excluded.chat_id,
                message_id = excluded.message_id,
                description = excluded.description,
                target_person_id = excluded.target_person_id,
                preview_message_id = excluded.preview_message_id,
                created = excluded.created;",
            new
            {
                id = draft.Id,
                team_id = draft.TeamId,
                strategy = draft.Strategy,
                chat_id = draft.ChatId,
                message_id = draft.MessageId,
                description = draft.Description,
                target_person_id = draft.TargetPersonId,
                preview_message_id = draft.PreviewMessageId,
                created = draft.Created
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }

    public async Task Delete(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            DELETE FROM review.draft_task_for_reviews AS d
            WHERE d.id = @id;",
            new { id },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}