using MediatR;

namespace Inc.TeamAssistant.Primitives.Commands;

public interface IContinuationCommand : IRequest<CommandResult>
{
    MessageContext MainContext { get; }
}