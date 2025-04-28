using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;

internal sealed class MoveToNextRoundCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.MoveToNextRound;
    
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

        return new MoveToNextRoundCommand(messageContext, messageContext.TryParseId(_command));
    }
}