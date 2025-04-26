using Inc.TeamAssistant.Appraiser.Model.Commands.FinishEstimate;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishEstimate.Services;

internal sealed class FinishEstimateCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.Finish;
    
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

        return Task.FromResult<IDialogCommand?>(new FinishEstimateCommand(
            messageContext,
            messageContext.TryParseId(_command)));
    }
}