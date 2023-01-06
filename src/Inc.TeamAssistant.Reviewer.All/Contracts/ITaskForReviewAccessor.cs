using Inc.TeamAssistant.Reviewer.All.Model;

namespace Inc.TeamAssistant.Reviewer.All.Contracts;

public interface ITaskForReviewAccessor
{
    Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        int limit,
        CancellationToken cancellationToken);

    Task Update(IReadOnlyCollection<TaskForReview> taskForReviews, CancellationToken cancellationToken);
}