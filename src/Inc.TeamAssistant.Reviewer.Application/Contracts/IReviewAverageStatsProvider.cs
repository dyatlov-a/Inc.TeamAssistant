using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IReviewAverageStatsProvider
{
    void Initialize(IReadOnlyCollection<TaskForReview> taskForReviews);

    void Add(TaskForReview taskForReview);

    ReviewAverageStats Get(Guid teamId);
}