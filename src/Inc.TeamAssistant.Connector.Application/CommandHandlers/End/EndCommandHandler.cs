using Inc.TeamAssistant.Connector.Model.Commands.End;
using Inc.TeamAssistant.Primitives.Commands;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.End;

internal sealed class EndCommandHandler : IRequestHandler<EndCommand, CommandResult>
{
    public Task<CommandResult> Handle(EndCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
            
        return Task.FromResult(CommandResult.Empty);
    }
}