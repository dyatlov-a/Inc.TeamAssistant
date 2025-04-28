using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Services;

internal sealed class JoinToTeamCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.Start;
    
    public IDialogCommand? TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return new JoinToTeamCommand(messageContext, messageContext.TryParseId(_command));
    }
}