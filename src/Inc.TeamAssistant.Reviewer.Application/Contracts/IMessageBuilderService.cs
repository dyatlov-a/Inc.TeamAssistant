using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IMessageBuilderService
{
    Task<string> NewTaskForReviewBuild(LanguageId languageId, TaskForReview taskForReview, CancellationToken token);
}