using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Services;

internal sealed class ReassignReviewCommandCreator : ICommandCreator
{
    public string Command => CommandList.ReassignReview;
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        return Task.FromResult<IDialogCommand>(new ReassignReviewCommand(
            messageContext,
            messageContext.TryParseId(Command)));
    }
}