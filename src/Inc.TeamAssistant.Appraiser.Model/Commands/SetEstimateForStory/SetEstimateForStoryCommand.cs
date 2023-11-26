using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;

public sealed record SetEstimateForStoryCommand(long AppraiserId, string AppraiserName, string Value)
	: IRequest<CommandResult>, IWithAppraiser;