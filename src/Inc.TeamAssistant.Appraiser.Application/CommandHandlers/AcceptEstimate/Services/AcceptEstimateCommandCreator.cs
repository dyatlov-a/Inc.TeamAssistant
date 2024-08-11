using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate.Services;

internal sealed class AcceptEstimateCommandCreator : ICommandCreator
{
    private readonly AssessmentValue.Value _value;
    public string Command { get; }
    public bool SupportSingleLineMode => false;

    public AcceptEstimateCommandCreator(string command, AssessmentValue.Value value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        
        Command = command;
        _value = value;
    }
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new AcceptEstimateCommand(
            messageContext,
            messageContext.TryParseId(Command),
            _value.ToString()));
    }
}