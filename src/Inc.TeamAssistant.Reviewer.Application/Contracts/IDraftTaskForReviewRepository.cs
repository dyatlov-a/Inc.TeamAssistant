using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IDraftTaskForReviewRepository
{
    Task<DraftTaskForReview?> Find(long chatId, int messageId, CancellationToken token);
    
    Task<DraftTaskForReview> GetById(Guid id, CancellationToken token);
    
    Task Upsert(DraftTaskForReview draft, CancellationToken token);
    
    Task Delete(Guid id, CancellationToken token);
}