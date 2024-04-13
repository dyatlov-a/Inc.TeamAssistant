using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandCreatorResolver
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;

    public CommandCreatorResolver(IEnumerable<ICommandCreator> commandCreators)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
    }

    public ICommandCreator? TryResolve(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(input));
        
        return _commandCreators.SingleOrDefault(c => input.StartsWith(
            c.Command,
            StringComparison.InvariantCultureIgnoreCase));
    }
}