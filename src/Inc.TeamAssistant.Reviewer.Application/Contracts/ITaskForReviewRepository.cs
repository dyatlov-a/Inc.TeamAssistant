using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewRepository
{
    Task<TaskForReview?> Find(Guid taskForReviewId, CancellationToken token);
    
    Task Upsert(TaskForReview taskForReview, CancellationToken token);
}