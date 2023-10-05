using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;

public sealed record ActivateAssessmentResult(
    AssessmentSessionId AssessmentSessionId,
    LanguageId AssessmentSessionLanguageId,
    string AssessmentSessionTitle);