using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IReviewMetricsLoader
{
    Task Load(IReadOnlyCollection<TaskForReview> taskForReviews, CancellationToken token);
}