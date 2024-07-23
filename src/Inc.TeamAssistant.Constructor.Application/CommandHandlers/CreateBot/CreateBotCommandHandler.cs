using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Domain;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateBot;

internal sealed class CreateBotCommandHandler : IRequestHandler<CreateBotCommand>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;
    private readonly IBotListeners _botListeners;
    private readonly IBotConnector _botConnector;

    public CreateBotCommandHandler(
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

    public async Task Handle(CreateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        
        var bot = new Bot(
            Guid.NewGuid(),
            command.Name,
            command.Token,
            currentPerson.Id,
            command.Properties,
            command.FeatureIds,
            command.SupportedLanguages);
        
        await _botRepository.Upsert(bot, token);

        await _botConnector.Update(bot.Id, command.BotDetails, token);
        await _botListeners.Start(bot.Id);
    }
}