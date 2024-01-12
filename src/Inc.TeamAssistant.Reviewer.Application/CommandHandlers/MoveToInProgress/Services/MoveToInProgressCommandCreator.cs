using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Services;

internal sealed class MoveToInProgressCommandCreator : ICommandCreator
{
    public string Command => CommandList.MoveToInProgress;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var storyId = Guid.Parse(messageContext.Text.Replace(Command, string.Empty));
        return Task.FromResult<IRequest<CommandResult>>(new MoveToInProgressCommand(messageContext, storyId));
    }
}