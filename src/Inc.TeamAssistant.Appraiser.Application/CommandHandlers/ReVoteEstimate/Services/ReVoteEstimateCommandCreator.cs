using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Services;

internal sealed class ReVoteEstimateCommandCreator : ICommandCreator
{
    public string Command => CommandList.ReVote;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IEndDialogCommand>(new ReVoteEstimateCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}