using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;

public sealed record MoveToInProgressCommand(MessageContext MessageContext, Guid TaskId)
    : IRequest<CommandResult>;