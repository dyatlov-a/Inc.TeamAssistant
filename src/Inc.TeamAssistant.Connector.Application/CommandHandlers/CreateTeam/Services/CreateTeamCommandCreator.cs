using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;

internal sealed class CreateTeamCommandCreator : ICommandCreator
{
    public string Command => CommandList.NewTeam;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new CreateTeamCommand(
            messageContext,
            messageContext.Bot.UserName,
            messageContext.Text));
    }
}