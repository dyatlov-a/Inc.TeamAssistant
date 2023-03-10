using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;

public sealed record CreateAssessmentSessionResult(LanguageId LanguageId, bool IsCreated);