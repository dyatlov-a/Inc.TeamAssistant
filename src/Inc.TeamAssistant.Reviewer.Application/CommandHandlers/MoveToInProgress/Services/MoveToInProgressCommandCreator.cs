using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Services;

internal sealed class MoveToInProgressCommandCreator : ICommandCreator
{
    public int Priority => 3;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        if (messageContext.Text.StartsWith(CommandList.MoveToInProgress, StringComparison.InvariantCultureIgnoreCase))
        {
            var storyId = Guid.Parse(messageContext.Text.Replace(CommandList.MoveToInProgress, string.Empty));
            return Task.FromResult<IRequest<CommandResult>?>(new MoveToInProgressCommand(messageContext, storyId));
        }

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}