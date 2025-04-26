using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.EditDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.EditDraft.Services;

internal sealed class EditDraftCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.EditDraft;
    
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

        var description = messageContext.Text.TrimStart(_command.ToArray());

        return Task.FromResult<IDialogCommand?>(new EditDraftCommand(messageContext, description));
    }
}