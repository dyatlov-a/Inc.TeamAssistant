using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;

public sealed record MoveToAcceptCommand(MessageContext MessageContext, Guid TaskId)
    : IRequest<CommandResult>;