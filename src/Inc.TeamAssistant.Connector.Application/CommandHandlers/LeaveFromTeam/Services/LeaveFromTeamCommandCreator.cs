using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;

internal sealed class LeaveFromTeamCommandCreator : ICommandCreator
{
    public string Command => CommandList.LeaveTeam;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new LeaveFromTeamCommand(
            messageContext,
            messageContext.TryParseId()));
    }
}