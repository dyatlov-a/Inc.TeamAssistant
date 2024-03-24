using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Services;

internal sealed class MoveToInProgressCommandCreator : ICommandCreator
{
    public string Command => CommandList.MoveToInProgress;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IEndDialogCommand>(new MoveToInProgressCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}