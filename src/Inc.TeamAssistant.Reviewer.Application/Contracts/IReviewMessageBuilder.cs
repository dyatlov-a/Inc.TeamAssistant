using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IReviewMessageBuilder
{
    Task<IReadOnlyCollection<NotificationMessage>> Build(
        int messageId,
        TaskForReview task,
        BotContext botContext,
        CancellationToken token);

    Task<NotificationMessage?> Push(TaskForReview task, CancellationToken token);

    Task<NotificationMessage> Build(DraftTaskForReview draft, LanguageId languageId, CancellationToken token);
}