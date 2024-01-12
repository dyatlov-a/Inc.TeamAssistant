using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;

internal sealed class MoveToDeclineCommandCreator : ICommandCreator
{
    public string Command => CommandList.Decline;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var storyId = Guid.Parse(messageContext.Text.Replace(Command, string.Empty));
        return Task.FromResult<IRequest<CommandResult>>(new MoveToDeclineCommand(messageContext, storyId));
    }
}