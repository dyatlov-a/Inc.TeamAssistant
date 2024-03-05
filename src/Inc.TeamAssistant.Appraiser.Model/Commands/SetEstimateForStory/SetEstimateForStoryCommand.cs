using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;

public sealed record SetEstimateForStoryCommand(MessageContext MessageContext, Guid StoryId, string Value)
	: IRequest<CommandResult>;