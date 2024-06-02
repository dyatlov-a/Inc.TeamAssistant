using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IReviewMetricsProvider
{
    Task<ReviewTeamMetrics> Create(TaskForReview taskForReview, CancellationToken token);
    
    Task Add(TaskForReview taskForReview, CancellationToken token);

    ReviewTeamMetrics Get(Guid teamId);
}