using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewReader
{
    Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        IReadOnlyCollection<TaskForReviewState> states,
        int limit,
        CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReview>> GetTasksByPerson(
        Guid teamId,
        long personId,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token);
    
    Task<bool> HasReassignFromDate(long personId, DateTimeOffset date, CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReview>> GetTasksFrom(Guid? teamId, DateTimeOffset date, CancellationToken token);
    
    Task<IReadOnlyDictionary<long, int>> GetHistory(Guid teamId, DateTimeOffset date, CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReviewDto>> GetLastTasks(Guid teamId, DateTimeOffset from, CancellationToken token);
}