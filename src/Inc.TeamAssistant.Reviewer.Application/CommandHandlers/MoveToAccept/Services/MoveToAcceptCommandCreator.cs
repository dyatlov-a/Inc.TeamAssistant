using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Services;

internal sealed class MoveToAcceptCommandCreator : ICommandCreator
{
    public string Command => CommandList.Accept;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext? teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var storyId = Guid.Parse(messageContext.Text.Replace(Command, string.Empty));
        return Task.FromResult<IRequest<CommandResult>>(new MoveToAcceptCommand(messageContext, storyId));
    }
}