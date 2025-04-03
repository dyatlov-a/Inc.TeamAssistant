using Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap.Services;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap;

internal sealed class AddLocationToMapCommandHandler : IRequestHandler<AddLocationToMapCommand, CommandResult>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly MapLinksBuilder _mapLinksBuilder;
    private readonly IMessageBuilder _messageBuilder;

    public AddLocationToMapCommandHandler(
        ILocationsRepository locationsRepository,
        MapLinksBuilder mapLinksBuilder,
        IMessageBuilder messageBuilder)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _mapLinksBuilder = mapLinksBuilder ?? throw new ArgumentNullException(nameof(mapLinksBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(AddLocationToMapCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var chatId = command.MessageContext.ChatMessage.ChatId;
        var personId = command.MessageContext.Person.Id;
        var languageId = command.MessageContext.LanguageId;
        
        if (chatId == personId)
        {
            var messageText = _messageBuilder.Build(Messages.CheckIn_GetStarted, languageId);

            return CommandResult.Build(NotificationMessage.Create(personId, messageText));
        }
        
        var existsMap = await _locationsRepository.Find(chatId, token);
        var map = existsMap ?? new(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            chatId,
            command.MessageContext.ChatName!);
        var location = new LocationOnMap(
            Guid.NewGuid(),
            personId,
            command.MessageContext.Person.DisplayName,
            command.MessageContext.Location!.X,
            command.MessageContext.Location.Y,
            map);

        await _locationsRepository.Insert(location, token);

        if (existsMap is not null)
            return CommandResult.Empty;

        var link = _mapLinksBuilder.Build(languageId, map.Id);
        var message = _messageBuilder.Build(Messages.CheckIn_ConnectLinkText, languageId, link);
            
        return CommandResult.Build(NotificationMessage.Create(chatId, message, pinned: true));
    }
}