using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.FinishStory;

public sealed record FinishStoryCommand(MessageContext MessageContext, Guid StoryId)
    : IEndDialogCommand;