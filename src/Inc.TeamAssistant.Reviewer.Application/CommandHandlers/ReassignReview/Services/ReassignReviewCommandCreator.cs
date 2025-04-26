using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Services;

internal sealed class ReassignReviewCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.ReassignReview;
    
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
        
        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult<IDialogCommand?>(null);

        
        return Task.FromResult<IDialogCommand?>(new ReassignReviewCommand(
            messageContext,
            messageContext.TryParseId(_command)));
    }
}