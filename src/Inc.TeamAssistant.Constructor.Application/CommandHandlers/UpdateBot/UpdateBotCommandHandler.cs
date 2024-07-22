using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateBot;

internal sealed class UpdateBotCommandHandler : IRequestHandler<UpdateBotCommand>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;
    private readonly IBotListeners _botListeners;
    private readonly IBotConnector _botConnector;

    public UpdateBotCommandHandler(
        IBotRepository botRepository,
        ICurrentPersonResolver currentPersonResolver,
        IBotListeners botListeners,
        IBotConnector botConnector)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentPersonResolver = currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
        _botListeners = botListeners ?? throw new ArgumentNullException(nameof(botListeners));
        _botConnector = botConnector ?? throw new ArgumentNullException(nameof(botConnector));
    }
    
    public async Task Handle(UpdateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        
        var bot = await _botRepository.FindById(command.Id, token);
        if (bot?.OwnerId != currentPerson.Id)
            throw new ApplicationException($"User {currentPerson.Id} has not access to bot {command.Id}.");
        
        bot
            .ChangeName(command.Name)
            .ChangeToken(command.Token)
            .ChangeFeatures(command.FeatureIds)
            .ChangeSupportedLanguages(command.SupportedLanguages);

        foreach (var property in command.Properties)
            bot.ChangeProperty(property.Key, property.Value);
        
        await _botRepository.Upsert(bot, token);

        await _botConnector.Setup(bot.Id, token);
        await _botListeners.Restart(bot.Id);
    }
}