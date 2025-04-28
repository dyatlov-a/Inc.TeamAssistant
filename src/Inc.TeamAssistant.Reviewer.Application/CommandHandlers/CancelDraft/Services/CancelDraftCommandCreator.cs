using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft.Services;

internal sealed class CancelDraftCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.RemoveDraft;
    
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

        return new CancelDraftCommand(messageContext, messageContext.TryParseId(_command));
    }
}