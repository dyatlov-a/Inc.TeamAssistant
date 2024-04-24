using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;

internal sealed class MoveToReviewCommandCreator : ICommandCreator
{
    public string Command => CommandList.NeedReview;
    public bool SupportSingleLineMode => true;
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new MoveToReviewCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.Properties.GetValueOrDefault("nextReviewerStrategy", NextReviewerType.RoundRobin.ToString()),
            messageContext.Text));
    }
}