using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IReviewTeamMetricsFactory
{
    Task<ReviewTeamMetrics> Create(ITaskForReviewStats taskForReview, CancellationToken token);
}