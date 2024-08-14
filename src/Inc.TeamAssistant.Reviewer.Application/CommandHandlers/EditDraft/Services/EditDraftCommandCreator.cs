using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.EditDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.EditDraft.Services;

internal sealed class EditDraftCommandCreator : ICommandCreator
{
    public string Command => CommandList.EditDraft;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        var description = messageContext.Text.TrimStart(Command.ToArray());

        return Task.FromResult<IDialogCommand>(new EditDraftCommand(messageContext, description));
    }
}