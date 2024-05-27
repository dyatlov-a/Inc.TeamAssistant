using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IMessageBuilderService
{
    Task<NotificationMessage> BuildNewTaskForReview(
        TaskForReview taskForReview,
        Person reviewer,
        Person owner,
        CancellationToken token);
    
    IAsyncEnumerable<NotificationMessage> BuildMoveToInProgress(
        TaskForReview task,
        Person reviewer,
        bool isPush,
        CancellationToken token);
    
    IAsyncEnumerable<NotificationMessage> BuildMoveToReviewActions(
        TaskForReview task,
        Person reviewer,
        bool isPush,
        bool hasActions,
        CancellationToken token);
    
    IAsyncEnumerable<NotificationMessage> BuildMoveToNextRound(
        TaskForReview task,
        Person owner,
        bool isPush,
        bool hasActions,
        CancellationToken token);

    Task<NotificationMessage> BuildReviewAccepted(
        TaskForReview task,
        Person owner,
        CancellationToken token);
}