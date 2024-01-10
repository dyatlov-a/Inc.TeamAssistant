using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Services;

internal sealed class ReVoteEstimateCommandCreator : ICommandCreator
{
    private readonly string _command = "/revote?storyId=";
    
    public int Priority => 3;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        if (messageContext.Text.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
        {
            var storyId = Guid.Parse(messageContext.Text.Replace(_command, string.Empty, StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult<IRequest<CommandResult>?>(new ReVoteEstimateCommand(messageContext, storyId));
        }

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}