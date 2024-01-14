using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

public sealed record AttachMessageCommand(MessageContext MessageContext, Guid TaskId, int MessageId)
    : IRequest<CommandResult>;