using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewRepository
{
    Task<TaskForReview> GetById(Guid taskForReviewId, CancellationToken token);

    Task Upsert(TaskForReview taskForReview, CancellationToken token);

    Task RetargetAndLeave(
        Guid teamId,
        long fromId,
        long toId,
        DateTimeOffset nextNotification,
        CancellationToken token);

    Task<long?> FindLastReviewer(Guid teamId, long ownerId, CancellationToken token);
}