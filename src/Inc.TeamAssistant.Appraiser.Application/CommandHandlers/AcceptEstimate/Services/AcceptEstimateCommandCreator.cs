using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;

internal sealed class AcceptEstimateCommandCreator : ICommandCreator
{
    private readonly string _command = "/accept?storyId=";
    
    public int Priority => 3;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        if (messageContext.Text.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
        {
            var storyId = Guid.Parse(messageContext.Text.Replace(_command, string.Empty));
            return Task.FromResult<IRequest<CommandResult>?>(new AcceptEstimateCommand(messageContext, storyId));
        }

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}