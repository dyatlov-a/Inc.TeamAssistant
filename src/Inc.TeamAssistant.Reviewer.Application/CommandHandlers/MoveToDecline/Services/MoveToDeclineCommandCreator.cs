using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;

internal sealed class MoveToDeclineCommandCreator : ICommandCreator
{
    public int Priority => 3;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        if (messageContext.Text.StartsWith(CommandList.Decline, StringComparison.InvariantCultureIgnoreCase))
        {
            var storyId = Guid.Parse(messageContext.Text.Replace(CommandList.Decline, string.Empty));
            return Task.FromResult<IRequest<CommandResult>?>(new MoveToDeclineCommand(messageContext, storyId));
        }

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}