using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.EditDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.EditDraft.Services;

internal sealed class EditDraftCommandCreator : ICommandCreator
{
    public string Command => CommandList.EditDraft;
    public bool SupportSingleLineMode => false;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var description = messageContext.Text.TrimStart(Command.ToArray());

        return Task.FromResult<IEndDialogCommand>(new EditDraftCommand(messageContext, description));
    }
}