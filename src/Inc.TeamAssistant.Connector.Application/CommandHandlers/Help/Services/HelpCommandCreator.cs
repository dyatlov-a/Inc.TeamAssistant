using Inc.TeamAssistant.Connector.Model.Commands.Help;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Help.Services;

internal sealed class HelpCommandCreator : ICommandCreator
{
    public string Command => "/help";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IRequest<CommandResult>>(new HelpCommand(messageContext));
    }
}