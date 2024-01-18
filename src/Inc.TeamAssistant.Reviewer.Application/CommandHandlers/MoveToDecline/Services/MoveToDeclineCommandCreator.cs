using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;

internal sealed class MoveToDeclineCommandCreator : ICommandCreator
{
    public string Command => CommandList.Decline;
    
    public Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IRequest<CommandResult>>(new MoveToDeclineCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}