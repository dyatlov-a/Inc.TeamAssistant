using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Services;

internal sealed class ReVoteEstimateCommandCreator : ICommandCreator
{
    public string Command => CommandList.ReVote;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IRequest<CommandResult>>(new ReVoteEstimateCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}