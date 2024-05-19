using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.RemoveBot;

internal sealed class RemoveBotCommandHandler : IRequestHandler<RemoveBotCommand>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentUserResolver _currentUserResolver;
    private readonly IBotListeners _botListeners;

    public RemoveBotCommandHandler(
        IBotRepository botRepository,
        ICurrentUserResolver currentUserResolver,
        IBotListeners botListeners)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentUserResolver = currentUserResolver ?? throw new ArgumentNullException(nameof(currentUserResolver));
        _botListeners = botListeners ?? throw new ArgumentNullException(nameof(botListeners));
    }

    public async Task Handle(RemoveBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentUserId = _currentUserResolver.GetUserId();
        var bot = await _botRepository.FindById(command.Id, token);
        if (bot?.OwnerId != currentUserId)
            throw new ApplicationException($"User {currentUserId} has not access to bot {command.Id}.");
        
        await _botRepository.Remove(command.Id, token);
        
        await _botListeners.Stop(bot.Id);
    }
}