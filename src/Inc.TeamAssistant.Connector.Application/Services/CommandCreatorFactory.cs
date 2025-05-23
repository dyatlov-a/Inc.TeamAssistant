using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandCreatorFactory
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;

    public CommandCreatorFactory(IEnumerable<ICommandCreator> commandCreators)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
    }

    public IDialogCommand? TryCreate(
        string input,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        foreach (var commandCreator in _commandCreators)
        {
            var command = commandCreator.TryCreate(
                input,
                singleLineMode,
                messageContext,
                teamContext);

            if (command is not null)
                return command;
        }

        return null;
    }
}