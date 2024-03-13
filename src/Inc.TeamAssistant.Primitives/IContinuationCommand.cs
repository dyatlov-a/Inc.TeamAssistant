using MediatR;

namespace Inc.TeamAssistant.Primitives;

public interface IContinuationCommand : IRequest<CommandResult>
{
    MessageContext MainContext { get; }
}