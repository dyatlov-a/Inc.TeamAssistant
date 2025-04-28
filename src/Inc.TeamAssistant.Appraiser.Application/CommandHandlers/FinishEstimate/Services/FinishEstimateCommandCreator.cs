using Inc.TeamAssistant.Appraiser.Model.Commands.FinishEstimate;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishEstimate.Services;

internal sealed class FinishEstimateCommandCreator : ICommandCreator
{
    private readonly string _command = CommandList.Finish;
    
    public IDialogCommand? TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        if (singleLineMode || !command.StartsWith(_command, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return new FinishEstimateCommand(messageContext, messageContext.TryParseId(_command));
    }
}