using Inc.TeamAssistant.Connector.Model.Commands.End;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.End.Services;

internal sealed class EndCommandCreator : ICommandCreator
{
    private readonly string _command = "/cancel";
    
    public int Priority => 2;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.CurrentCommandStage.HasValue &&
            messageContext.Text.Equals(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IRequest<CommandResult>?>(new EndCommand(
                messageContext,
                messageContext.CurrentCommandStage.Value));
        
        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}