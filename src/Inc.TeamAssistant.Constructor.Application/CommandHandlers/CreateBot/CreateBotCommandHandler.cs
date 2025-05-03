using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Domain;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateBot;

internal sealed class CreateBotCommandHandler : IRequestHandler<CreateBotCommand>
{
    private readonly IBotRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IBotListeners _botListeners;
    private readonly IBotConnector _botConnector;

    public CreateBotCommandHandler(
        IBotRepository repository,
        IPersonResolver personResolver,
        IBotListeners botListeners,
        IBotConnector botConnector)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _botListeners = botListeners ?? throw new ArgumentNullException(nameof(botListeners));
        _botConnector = botConnector ?? throw new ArgumentNullException(nameof(botConnector));
    }

    public async Task Handle(CreateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var bot = new Bot(
            Guid.NewGuid(),
            command.Name,
            command.Token,
            currentPerson.Id,
            command.CalendarId,
            command.Properties,
            command.FeatureIds,
            command.SupportedLanguages);
        
        await _repository.Upsert(bot, token);
        await _botConnector.SetCommands(bot.Id, bot.SupportedLanguages, token);
        await _botListeners.Start(bot.Id, token);
    }
}