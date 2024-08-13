using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;

internal sealed class JoinToTeamCommandCreator : ICommandCreator
{
    public string Command => CommandList.Start;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new JoinToTeamCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}