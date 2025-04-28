using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

public sealed class ReviewCommentsProvider
{
    private readonly Dictionary<(long ChatId, int MessageId), Guid> _data = new();

    public Guid? Check(long chatId, int messageId) => _data.TryGetValue((chatId, messageId), out var value)
        ? value
        : null;
    
    public void Add(TaskForReview task)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        if (task.MessageId.HasValue)
            AddOrUpdate(task.ChatId, task.MessageId.Value, task.Id);
        if (task.OwnerMessageId.HasValue)
            AddOrUpdate(task.OwnerId, task.OwnerMessageId.Value, task.Id);
        if (task.ReviewerMessageId.HasValue)
            AddOrUpdate(task.ReviewerId, task.ReviewerMessageId.Value, task.Id);
        if (task is { OriginalReviewerId: not null, OriginalReviewerMessageId: not null })
            AddOrUpdate(task.OriginalReviewerId.Value, task.OriginalReviewerMessageId.Value, task.Id);
        if (task is { FirstReviewerId: not null, FirstReviewerMessageId: not null })
            AddOrUpdate(task.FirstReviewerId.Value, task.FirstReviewerMessageId.Value, task.Id);
    }

    public void Remove(TaskForReview task)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        if (task.MessageId.HasValue)
            Remove(task.ChatId, task.MessageId.Value);
        if (task.OwnerMessageId.HasValue)
            Remove(task.OwnerId, task.OwnerMessageId.Value);
        if (task.ReviewerMessageId.HasValue)
            Remove(task.ReviewerId, task.ReviewerMessageId.Value);
        if (task is { OriginalReviewerId: not null, OriginalReviewerMessageId: not null })
            Remove(task.OriginalReviewerId.Value, task.OriginalReviewerMessageId.Value);
        if (task is { FirstReviewerId: not null, FirstReviewerMessageId: not null })
            Remove(task.FirstReviewerId.Value, task.FirstReviewerMessageId.Value);
    }

    private void AddOrUpdate(long chatId, int messageId, Guid taskId) => _data[(chatId, messageId)] = taskId;

    private void Remove(long chatId, int messageId) => _data.Remove((chatId, messageId));
}