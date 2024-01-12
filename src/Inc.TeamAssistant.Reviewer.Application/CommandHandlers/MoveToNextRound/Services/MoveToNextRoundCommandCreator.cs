using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Services;

internal sealed class MoveToNextRoundCommandCreator : ICommandCreator
{
    public string Command => CommandList.MoveToNextRound;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var storyId = Guid.Parse(messageContext.Text.Replace(Command, string.Empty));
        return Task.FromResult<IRequest<CommandResult>>(new MoveToNextRoundCommand(messageContext, storyId));
    }
}