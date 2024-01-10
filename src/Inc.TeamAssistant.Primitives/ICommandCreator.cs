using MediatR;

namespace Inc.TeamAssistant.Primitives;

public interface ICommandCreator
{
    int Priority { get; }
    
    Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token);
}