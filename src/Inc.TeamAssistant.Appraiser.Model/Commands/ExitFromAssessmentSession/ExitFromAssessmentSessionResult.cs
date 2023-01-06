using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;

public sealed record ExitFromAssessmentSessionResult(
    AssessmentSessionDetails AssessmentSessionDetails,
    ParticipantId ModeratorId,
	string AppraiserName);