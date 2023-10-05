using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;

public sealed record FinishAssessmentSessionResult(
    AssessmentSessionId AssessmentSessionId,
    LanguageId AssessmentSessionLanguageId,
	string AssessmentSessionTitle,
	IReadOnlyCollection<ParticipantId> AppraiserIds);