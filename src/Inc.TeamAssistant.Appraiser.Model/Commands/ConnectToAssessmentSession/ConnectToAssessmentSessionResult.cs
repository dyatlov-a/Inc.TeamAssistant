using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;

public sealed record ConnectToAssessmentSessionResult(
	ParticipantId ModeratorId,
    AssessmentSessionId AssessmentSessionId,
    LanguageId AssessmentSessionLanguageId,
	string AssessmentSessionTitle,
	string AppraiserName,
	bool StoryInProgress);