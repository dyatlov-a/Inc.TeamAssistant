using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;

public sealed record AddStoryForEstimateCommand(
        AssessmentSessionId AssessmentSessionId,
		long TargetChatId,
		string AppraiserName,
		int StoryExternalId,
		bool IsUpdate)
	: IRequest<AddStoryForEstimateResult>, IWithAppraiser;