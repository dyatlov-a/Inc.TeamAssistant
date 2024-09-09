using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandCreatorResolver
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;

    public CommandCreatorResolver(IEnumerable<ICommandCreator> commandCreators)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
    }

    public ICommandCreator? TryResolve(string input, bool onlySingleLineCommand = false)
    {
        ArgumentNullException.ThrowIfNull(input);

        return _commandCreators.SingleOrDefault(c => (!onlySingleLineCommand || c.SupportSingleLineMode) &&
            input.StartsWith(c.Command, StringComparison.InvariantCultureIgnoreCase));
    }
}