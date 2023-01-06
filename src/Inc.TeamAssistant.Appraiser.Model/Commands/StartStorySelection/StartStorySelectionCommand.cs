using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;

public sealed record StartStorySelectionCommand(ParticipantId ModeratorId, string ModeratorName)
	: IRequest<StartStorySelectionResult>, IWithModerator;