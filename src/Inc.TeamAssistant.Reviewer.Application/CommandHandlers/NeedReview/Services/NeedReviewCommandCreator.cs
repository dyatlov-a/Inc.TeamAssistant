using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.NeedReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.NeedReview.Services;

internal sealed class NeedReviewCommandCreator : ICommandCreator
{
    public string Command => CommandList.NeedReview;
    public bool SupportSingleLineMode => true;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new NeedReviewCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.GetNextReviewerType(),
            messageContext.Text));
    }
}