using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IReviewMessageBuilder
{
    Task<IReadOnlyCollection<NotificationMessage>> Build(
        int messageId,
        TaskForReview taskForReview,
        Person reviewer,
        Person owner,
        CancellationToken token);

    Task<NotificationMessage?> Push(TaskForReview taskForReview, CancellationToken token);

    Task<NotificationMessage> Build(DraftTaskForReview draft, CancellationToken token);
}