using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.RemoveBot;

internal sealed class RemoveBotCommandHandler : IRequestHandler<RemoveBotCommand>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;
    private readonly IBotListeners _botListeners;

    public RemoveBotCommandHandler(
        IBotRepository botRepository,
        ICurrentPersonResolver currentPersonResolver,
        IBotListeners botListeners)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentPersonResolver = currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
        _botListeners = botListeners ?? throw new ArgumentNullException(nameof(botListeners));
    }

    public async Task Handle(RemoveBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        
        var bot = await _botRepository.FindById(command.Id, token);
        if (bot?.OwnerId != currentPerson.Id)
            throw new ApplicationException($"User {currentPerson.Id} has not access to bot {command.Id}.");
        
        await _botRepository.Remove(command.Id, token);
        
        await _botListeners.Stop(bot.Id);
    }
}