using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Services;

internal sealed class MoveToAcceptCommandCreator : ICommandCreator
{
    public string Command => CommandList.Accept;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new MoveToAcceptCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}