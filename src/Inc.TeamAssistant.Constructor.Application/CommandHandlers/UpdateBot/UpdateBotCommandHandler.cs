using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateBot;

internal sealed class UpdateBotCommandHandler : IRequestHandler<UpdateBotCommand>
{
    private readonly IBotRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IBotListeners _botListeners;
    private readonly IBotConnector _botConnector;

    public UpdateBotCommandHandler(
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
    
    public async Task Handle(UpdateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _personResolver.GetCurrentPerson();
        var bot = await command.Id.Required(_repository.Find, token);
        
        bot
            .CheckRights(currentPerson.Id)
            .ChangeName(command.Name)
            .ChangeToken(command.Token)
            .ChangeCalendarId(command.CalendarId)
            .ChangeFeatures(command.FeatureIds)
            .ChangeSupportedLanguages(command.SupportedLanguages)
            .ChangeProperties(command.Properties);
        
        await _repository.Upsert(bot, token);
        await _botConnector.SetCommands(bot.Id, bot.SupportedLanguages, token);
        await _botListeners.Restart(bot.Id, token);
    }
}