using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;

public sealed record FinishAssessmentSessionResult(
    AssessmentSessionId AssessmentSessionId,
    LanguageId AssessmentSessionLanguageId,
	string AssessmentSessionTitle,
	IReadOnlyCollection<ParticipantId> AppraiserIds);