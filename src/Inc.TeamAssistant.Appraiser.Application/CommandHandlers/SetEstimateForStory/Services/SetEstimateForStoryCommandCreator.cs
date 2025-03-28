using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Services;

internal sealed class SetEstimateForStoryCommandCreator : ICommandCreator
{
    private readonly int _value;
    
    public string Command { get; }

    public SetEstimateForStoryCommandCreator(string command, int value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        
        Command = command;
        _value = value;
    }
    
    public Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IDialogCommand>(new SetEstimateForStoryCommand(
            messageContext,
            messageContext.TryParseId(Command),
            _value));
    }
}