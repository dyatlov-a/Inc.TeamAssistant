using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Services;

internal sealed class ReVoteEstimateCommandCreator : ICommandCreator
{
    public string Command => "/revote?storyId=";
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var storyId = Guid.Parse(messageContext.Text.Replace(Command, string.Empty, StringComparison.InvariantCultureIgnoreCase));
        return Task.FromResult<IRequest<CommandResult>>(new ReVoteEstimateCommand(messageContext, storyId));
    }
}