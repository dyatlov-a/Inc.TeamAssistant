using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;

public sealed record MoveToDeclineCommand(MessageContext MessageContext, Guid TaskId)
    : IRequest<CommandResult>;