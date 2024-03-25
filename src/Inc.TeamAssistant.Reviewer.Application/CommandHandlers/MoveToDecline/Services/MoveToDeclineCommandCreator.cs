using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Services;

internal sealed class MoveToDeclineCommandCreator : ICommandCreator
{
    public string Command => CommandList.Decline;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
        
        return Task.FromResult<IEndDialogCommand>(new MoveToDeclineCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}