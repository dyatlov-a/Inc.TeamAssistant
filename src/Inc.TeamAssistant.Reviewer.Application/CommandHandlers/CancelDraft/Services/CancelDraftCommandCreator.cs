using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft.Services;

internal sealed class CancelDraftCommandCreator : ICommandCreator
{
    public string Command => CommandList.RemoveDraft;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new CancelDraftCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}