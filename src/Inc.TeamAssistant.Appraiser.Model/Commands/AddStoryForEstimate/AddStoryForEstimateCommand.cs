using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;

public sealed record AddStoryForEstimateCommand(
        AssessmentSessionId AssessmentSessionId,
        string ModeratorName,
		int StoryExternalId)
	: IRequest<CommandResult>, IWithModerator;