using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;

internal sealed class MoveToReviewCommandCreator : ICommandCreator
{
    public string Command => CommandList.MoveToReview;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new MoveToReviewCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}