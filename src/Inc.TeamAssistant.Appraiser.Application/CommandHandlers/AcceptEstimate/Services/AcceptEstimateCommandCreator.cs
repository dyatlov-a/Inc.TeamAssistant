using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;

internal sealed class AcceptEstimateCommandCreator : ICommandCreator
{
    private readonly int _value;
    private readonly string _command;

    public AcceptEstimateCommandCreator(string command, int value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        
        _command = command;
        _value = value;
    }
    
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
        
        return new AcceptEstimateCommand(messageContext, messageContext.TryParseId(_command), _value);
    }
}