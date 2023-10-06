using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewRepository
{
    Task<IReadOnlyCollection<Guid>> GetTaskIds(
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken cancellationToken);

    Task<TaskForReview> GetById(Guid taskForReviewId, CancellationToken cancellationToken);

    Task Upsert(TaskForReview taskForReview, CancellationToken cancellationToken);

    Task RetargetAndLeave(
        Guid teamId,
        Person from,
        Person to,
        DateTimeOffset nextNotification,
        CancellationToken cancellationToken);
}