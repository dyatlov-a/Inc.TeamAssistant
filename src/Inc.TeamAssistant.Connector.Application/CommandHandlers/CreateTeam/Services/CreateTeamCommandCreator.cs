using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;

internal sealed class CreateTeamCommandCreator : ICommandCreator
{
    public string Command => CommandList.NewTeam;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
            
        return Task.FromResult<IEndDialogCommand>(new CreateTeamCommand(
            messageContext,
            messageContext.BotName,
            messageContext.Text,
            teamContext.Properties));
    }
}