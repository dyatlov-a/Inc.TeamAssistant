using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record AssessmentSessionDetails(
    AssessmentSessionId Id,
    string Title,
    LanguageId LanguageId,
    StoryDetails Story,
    IReadOnlyCollection<EstimateItemDetails> Items);