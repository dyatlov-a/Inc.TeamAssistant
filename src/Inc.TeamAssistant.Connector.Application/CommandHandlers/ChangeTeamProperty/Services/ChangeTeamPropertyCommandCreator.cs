using Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.ChangeTeamProperty.Services;

internal sealed class ChangeTeamPropertyCommandCreator : ICommandCreator
{
    private readonly string _propertyName;
    private readonly string _propertyValue;
    
    public string Command { get; }
    public bool SupportSingleLineMode => false;

    public ChangeTeamPropertyCommandCreator(string command, string propertyName, string propertyValue)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));
        if (string.IsNullOrWhiteSpace(propertyValue))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyValue));
        
        Command = command;
        _propertyName = propertyName;
        _propertyValue = propertyValue;
    }
    
    public Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        return Task.FromResult<IEndDialogCommand>(new ChangeTeamPropertyCommand(
            messageContext,
            messageContext.TryParseId("/"),
            _propertyName,
            _propertyValue));
    }
}