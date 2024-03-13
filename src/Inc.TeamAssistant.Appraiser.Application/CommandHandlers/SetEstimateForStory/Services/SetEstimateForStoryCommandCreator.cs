using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Services;

internal sealed class SetEstimateForStoryCommandCreator : ICommandCreator
{
    private readonly AssessmentValue.Value _value;
    
    public string Command { get; }

    public SetEstimateForStoryCommandCreator(string command, AssessmentValue.Value value)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));
        
        Command = command;
        _value = value;
    }
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teamContext is null)
            throw new ArgumentNullException(nameof(teamContext));
                
        return Task.FromResult<IEndDialogCommand>(new SetEstimateForStoryCommand(
            messageContext,
            messageContext.TryParseId(Command),
            _value.ToString()));
    }
}