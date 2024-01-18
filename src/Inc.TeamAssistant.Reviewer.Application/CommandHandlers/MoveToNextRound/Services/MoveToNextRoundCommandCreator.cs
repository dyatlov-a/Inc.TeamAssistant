using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;

internal sealed class MoveToNextRoundCommandCreator : ICommandCreator
{
    public string Command => CommandList.MoveToNextRound;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IRequest<CommandResult>>(new MoveToNextRoundCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}