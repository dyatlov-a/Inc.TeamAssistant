using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;

public sealed record SetEstimateForStoryCommand(ParticipantId AppraiserId, string AppraiserName, int? Value)
	: IRequest<SetEstimateForStoryResult>, IWithAppraiser;