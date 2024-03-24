using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;

public sealed record AttachStoryCommand(MessageContext MainContext, Guid StoryId, int MessageId)
    : IContinuationCommand;