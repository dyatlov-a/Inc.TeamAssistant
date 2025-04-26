using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft.Services;

internal sealed class CancelDraftCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.RemoveDraft;
    
    public Task<IDialogCommand?> TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IDialogCommand?>(null);

        return Task.FromResult<IDialogCommand?>(new CancelDraftCommand(
            messageContext,
            messageContext.TryParseId(_command)));
    }
}