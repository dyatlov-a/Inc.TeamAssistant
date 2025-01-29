using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeam;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeam.Services;

internal sealed class RemoveTeamCommandCreator : ICommandCreator
{
    public string Command => CommandList.RemoveTeam;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new RemoveTeamCommand(
            messageContext,
            messageContext.TryParseId()));
    }
}