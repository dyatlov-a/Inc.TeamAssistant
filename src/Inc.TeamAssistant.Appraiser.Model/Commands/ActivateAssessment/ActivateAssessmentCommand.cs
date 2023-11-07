using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;

public sealed record ActivateAssessmentCommand(
		long TargetChatId,
		ParticipantId ModeratorId,
		string ModeratorName,
		string Title)
	: IRequest<CommandResult>, IWithModerator;