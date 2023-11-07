using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;

public sealed record StartStorySelectionCommand(long TargetChatId, ParticipantId ModeratorId, string ModeratorName)
	: IRequest<CommandResult>, IWithModerator;