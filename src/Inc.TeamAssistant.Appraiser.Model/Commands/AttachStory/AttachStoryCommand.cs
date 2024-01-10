using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;

public sealed record AttachStoryCommand(MessageContext MessageContext, Guid StoryId, int MessageId)
    : IRequest<CommandResult>;