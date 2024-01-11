using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;

internal sealed class MoveToNextRoundCommandCreator : ICommandCreator
{
    public int Priority => 3;
    
    public Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        if (messageContext.Text.StartsWith(CommandList.MoveToNextRound, StringComparison.InvariantCultureIgnoreCase))
        {
            var storyId = Guid.Parse(messageContext.Text.Replace(CommandList.MoveToNextRound, string.Empty));
            return Task.FromResult<IRequest<CommandResult>?>(new MoveToNextRoundCommand(messageContext, storyId));
        }

        return Task.FromResult<IRequest<CommandResult>?>(null);
    }
}