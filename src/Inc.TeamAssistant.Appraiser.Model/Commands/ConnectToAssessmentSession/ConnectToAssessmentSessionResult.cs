using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;

public sealed record ConnectToAssessmentSessionResult(
	ParticipantId ModeratorId,
	string AppraiserName,
	bool IsModerator,
	string AssessmentSessionTitle,
	bool InProgress,
	SummaryByStory SummaryByStory);