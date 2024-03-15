using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;

public sealed record AttachStoryCommand(MessageContext MainContext, Guid StoryId, int MessageId)
    : IContinuationCommand;