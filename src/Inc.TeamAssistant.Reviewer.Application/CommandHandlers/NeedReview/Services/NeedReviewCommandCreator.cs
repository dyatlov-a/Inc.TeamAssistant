using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.NeedReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.NeedReview.Services;

internal sealed class NeedReviewCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.NeedReview;
    
    public Task<IDialogCommand?> TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        if (!command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IDialogCommand?>(null);

        return Task.FromResult<IDialogCommand?>(new NeedReviewCommand(
            messageContext,
            teamContext.TeamId,
            teamContext.GetNextReviewerType(),
            messageContext.Text));
    }
}