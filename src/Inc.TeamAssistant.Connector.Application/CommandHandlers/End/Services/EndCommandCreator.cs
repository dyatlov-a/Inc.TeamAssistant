using Inc.TeamAssistant.Connector.Model.Commands.End;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.End.Services;

internal sealed class EndCommandCreator : ICommandCreator
{
    public string Command => CommandList.Cancel;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IEndDialogCommand>(new EndCommand(messageContext));
    }
}