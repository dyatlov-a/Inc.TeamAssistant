using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.RemoveBot;

internal sealed class RemoveBotCommandHandler : IRequestHandler<RemoveBotCommand>
{
    private readonly IBotRepository _botRepository;

    public RemoveBotCommandHandler(IBotRepository botRepository)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task Handle(RemoveBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var bot = await _botRepository.FindById(command.Id, token);
        if (bot?.OwnerId != command.CurrentUserId)
            throw new ApplicationException($"User {command.CurrentUserId} has not access to bot {command.Id}.");
        
        await _botRepository.Remove(command.Id, token);
    }
}