using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewReader
{
    Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReview>> GetTasksByPerson(
        Guid teamId,
        long personId,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReview>> GetTasksFrom(Guid? teamId, DateTimeOffset date, CancellationToken token);
    
    Task<IReadOnlyDictionary<long, int>> GetHistory(Guid teamId, DateTimeOffset date, CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReviewHistory>> GetLastTasks(
        Guid teamId,
        DateTimeOffset from,
        CancellationToken token);
    
    Task<IReadOnlyCollection<ReviewTicket>> GetLastFirstReviewers(Guid teamId, CancellationToken token);

    Task<long?> GetLastSecondReviewer(Guid teamId, CancellationToken token);
}