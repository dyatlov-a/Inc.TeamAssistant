using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IMessageBuilderService
{
    Task<NotificationMessage> BuildMessageNewTaskForReview(TaskForReview taskForReview, Person reviewer, Person owner);
    Task<NotificationMessage> BuildMessageNeedReview(TaskForReview task, Person reviewer);
    Task<NotificationMessage> BuildMessageMoveToNextRound(TaskForReview task, Person owner);
}