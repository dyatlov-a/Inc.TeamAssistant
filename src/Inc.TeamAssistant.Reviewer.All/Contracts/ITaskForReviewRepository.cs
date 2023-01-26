using Inc.TeamAssistant.Reviewer.All.Model;

namespace Inc.TeamAssistant.Reviewer.All.Contracts;

public interface ITaskForReviewRepository
{
    Task<IReadOnlyCollection<Guid>> GetTaskIds(
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken cancellationToken);

    Task<TaskForReview> GetById(Guid taskForReviewId, CancellationToken cancellationToken);

    Task Upsert(TaskForReview taskForReview, CancellationToken cancellationToken);
}