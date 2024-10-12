using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Services;

internal sealed class MoveToAcceptWithCommentsCommandCreator : ICommandCreator
{
    public string Command => CommandList.AcceptWithComments;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new MoveToAcceptCommand(
            messageContext,
            messageContext.TryParseId(Command),
            true));
    }
}