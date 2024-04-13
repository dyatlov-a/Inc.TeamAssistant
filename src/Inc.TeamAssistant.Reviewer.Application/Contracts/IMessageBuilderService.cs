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
    
    Task<NotificationMessage> BuildNeedReview(
        TaskForReview task,
        Person reviewer,
        bool? hasInProgressAction,
        ChatMessage? chatMessage,
        CancellationToken token);
    
    Task<NotificationMessage> BuildMoveToNextRound(
        TaskForReview task,
        Person owner,
        ChatMessage? chatMessage,
        CancellationToken token);

    Task<NotificationMessage> BuildReviewAccepted(
        TaskForReview task,
        Person owner,
        CancellationToken token);
}