using Inc.TeamAssistant.Connector.Model.Commands.Help;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Help.Services;

internal sealed class HelpCommandCreator : ICommandCreator
{
    private readonly string _command = "/help";
    
    public int Priority => 1;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.Text.Equals(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IRequest<CommandResult>?>(new HelpCommand(messageContext));

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}