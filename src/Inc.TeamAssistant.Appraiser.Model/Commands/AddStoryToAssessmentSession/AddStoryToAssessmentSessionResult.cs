using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;

public sealed record AddStoryToAssessmentSessionResult(
    AssessmentSessionId AssessmentSessionId,
    long ChatId,
    LanguageId AssessmentSessionLanguageId,
    StoryDetails Story,
    IReadOnlyCollection<EstimateItemDetails> Items);