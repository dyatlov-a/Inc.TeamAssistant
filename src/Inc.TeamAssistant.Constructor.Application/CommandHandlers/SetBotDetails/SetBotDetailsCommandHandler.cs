using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.SetBotDetails;

internal sealed class SetBotDetailsCommandHandler : IRequestHandler<SetBotDetailsCommand>
{
    private readonly IBotConnector _botConnector;

    public SetBotDetailsCommandHandler(IBotConnector botConnector)
    {
        _botConnector = botConnector ?? throw new ArgumentNullException(nameof(botConnector));
    }

    public async Task Handle(SetBotDetailsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _botConnector.SetDetails(command.Token, command.BotDetails, token);
    }
}