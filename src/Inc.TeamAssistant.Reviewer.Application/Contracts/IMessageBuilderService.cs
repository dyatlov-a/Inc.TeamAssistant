using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Domain;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IMessageBuilderService
{
    Task<(string Text, IReadOnlyCollection<MessageEntity> Entities)> NewTaskForReviewBuild(
        LanguageId languageId,
        TaskForReview taskForReview,
        CancellationToken token);
}