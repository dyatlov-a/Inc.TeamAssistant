using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandCreatorResolver
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;

    public CommandCreatorResolver(IEnumerable<ICommandCreator> commandCreators)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
    }

    public ICommandCreator? TryResolve(string command)
    {
        return _commandCreators.SingleOrDefault(c => command.Equals(
            c.Command,
            StringComparison.InvariantCultureIgnoreCase));
    }
}