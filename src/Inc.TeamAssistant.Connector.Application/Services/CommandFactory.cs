using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandFactory
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;

    public CommandFactory(IEnumerable<ICommandCreator> commandCreators)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
    }

    public async Task<IRequest<CommandResult>?> TryCreate(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        foreach (var commandCreator in _commandCreators.OrderBy(c => c.Priority))
        {
            var command = await commandCreator.Create(messageContext, token);
            if (command is not null)
                return command;
        }
        
        return null;
    }
}