using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;

public sealed record SetEstimateForStoryCommand(MessageContext MessageContext, Guid StoryId, string Value)
	: IEndDialogCommand;