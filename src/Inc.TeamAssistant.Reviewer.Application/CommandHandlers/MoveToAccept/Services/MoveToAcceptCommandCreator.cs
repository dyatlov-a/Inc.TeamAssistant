using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Services;

internal sealed class MoveToAcceptCommandCreator : ICommandCreator
{
    public string Command => CommandList.Accept;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IEndDialogCommand>(new MoveToAcceptCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}