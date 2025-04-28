using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Services;

internal sealed class MoveToInProgressCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.MoveToInProgress;
    
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

        return new MoveToInProgressCommand(messageContext, messageContext.TryParseId(_command));
    }
}