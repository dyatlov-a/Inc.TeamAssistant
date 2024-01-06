using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;

public sealed record StartStorySelectionCommand(long TargetChatId, long ModeratorId, string ModeratorName)
	: IRequest<CommandResult>, IWithModerator;