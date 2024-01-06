using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;

public sealed record AddStoryForEstimateCommand(
		Guid AssessmentSessionId,
        string ModeratorName,
		int StoryExternalId)
	: IRequest<CommandResult>, IWithModerator;