using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewAccessor
{
    Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        IReadOnlyCollection<TaskForReviewState> states,
        int limit,
        CancellationToken cancellationToken);

    Task Update(IReadOnlyCollection<TaskForReview> taskForReviews, CancellationToken cancellationToken);
}