using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.SetBotDetails;

internal sealed class SetBotDetailsCommandHandler : IRequestHandler<SetBotDetailsCommand>
{
    private readonly IBotConnector _connector;

    public SetBotDetailsCommandHandler(IBotConnector connector)
    {
        _connector = connector ?? throw new ArgumentNullException(nameof(connector));
    }

    public async Task Handle(SetBotDetailsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _connector.SetDetails(command.Token, command.BotDetails, token);
    }
}