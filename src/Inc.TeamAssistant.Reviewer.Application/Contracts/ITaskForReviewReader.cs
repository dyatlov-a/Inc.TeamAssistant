using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface ITaskForReviewReader
{
    Task<IReadOnlyCollection<TaskForReview>> GetAll(
        IReadOnlyCollection<TaskForReviewState> states,
        Guid? teamId,
        CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReview>> GetTasksForNotifications(
        DateTimeOffset now,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReview>> GetTasksByPerson(
        Guid teamId,
        long personId,
        IReadOnlyCollection<TaskForReviewState> states,
        CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReview>> GetTasksByTeam(Guid? teamId, DateTimeOffset date, CancellationToken token);
    
    Task<IReadOnlyCollection<TaskForReviewHistory>> GetLastTasks(
        Guid teamId,
        DateTimeOffset from,
        CancellationToken token);

    Task<ReviewerCandidatePool> GetReviewerCandidates(
        Guid teamId,
        DateTimeOffset fromDate,
        IReadOnlyCollection<TaskForReviewState> states,
        NextReviewerType excludeType,
        CancellationToken token);
}