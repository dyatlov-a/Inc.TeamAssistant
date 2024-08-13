using Inc.TeamAssistant.Connector.Model.Commands.Help;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Help.Services;

internal sealed class HelpCommandCreator : ICommandCreator
{
    public string Command => CommandList.Help;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new HelpCommand(messageContext));
    }
}