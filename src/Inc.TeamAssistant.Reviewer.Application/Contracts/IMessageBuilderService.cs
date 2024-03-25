using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IMessageBuilderService
{
    Task<NotificationMessage> BuildMessageNewTaskForReview(
        TaskForReview taskForReview,
        Person reviewer,
        Person owner,
        CancellationToken token);
    Task<NotificationMessage> BuildMessageNeedReview(TaskForReview task, Person reviewer, CancellationToken token);
    Task<NotificationMessage> BuildMessageMoveToNextRound(TaskForReview task, Person owner, CancellationToken token);
}