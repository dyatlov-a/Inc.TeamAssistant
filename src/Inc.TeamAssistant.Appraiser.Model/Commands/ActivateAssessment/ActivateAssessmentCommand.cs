using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;

public sealed record ActivateAssessmentCommand(
		long TargetChatId,
		long ModeratorId,
		string ModeratorName,
		string Title)
	: IRequest<CommandResult>, IWithModerator;