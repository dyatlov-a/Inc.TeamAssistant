using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;

public sealed record MoveToNextRoundCommand(MessageContext MessageContext, Guid TaskId)
    : IRequest<CommandResult>;