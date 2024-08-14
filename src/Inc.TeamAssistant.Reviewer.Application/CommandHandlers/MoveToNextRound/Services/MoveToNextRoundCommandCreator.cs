using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;

internal sealed class MoveToNextRoundCommandCreator : ICommandCreator
{
    public string Command => CommandList.MoveToNextRound;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new MoveToNextRoundCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}