using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;

public sealed record ConnectToAssessmentSessionResult(
	ParticipantId ModeratorId,
	LanguageId AssessmentSessionLanguageId,
	string AssessmentSessionTitle,
	ParticipantId AppraiserId,
	string AppraiserName);