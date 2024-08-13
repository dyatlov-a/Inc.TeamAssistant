using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft.Services;

internal sealed class CancelDraftCommandCreator : ICommandCreator
{
    public string Command => CommandList.RemoveDraft;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new CancelDraftCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}