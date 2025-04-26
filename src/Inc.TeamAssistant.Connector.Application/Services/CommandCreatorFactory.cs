using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandCreatorFactory
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;

    public CommandCreatorFactory(IEnumerable<ICommandCreator> commandCreators)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
    }

    public async Task<IDialogCommand?> TryCreate(
        string input,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);

        foreach (var commandCreator in _commandCreators)
        {
            var command = await commandCreator.TryCreate(
                input,
                singleLineMode,
                messageContext,
                teamContext,
                token);

            if (command is not null)
                return command;
        }

        return null;
    }
}