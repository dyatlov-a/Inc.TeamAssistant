using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewRepository
{
    Task<IReadOnlyCollection<TaskForReview>> Get(
        Guid teamId,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token);
    
    Task<TaskForReview> GetById(Guid taskForReviewId, CancellationToken token);
    
    Task Upsert(TaskForReview taskForReview, CancellationToken token);
    
    Task<IReadOnlyCollection<ReviewTicket>> GetLastReviewers(Guid teamId, CancellationToken token);
}