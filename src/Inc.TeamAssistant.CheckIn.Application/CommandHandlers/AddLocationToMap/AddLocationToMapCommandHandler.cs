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
        
        if (command.MessageContext.ChatMessage.ChatId == command.MessageContext.Person.Id)
        {
            var messageText = await _messageBuilder.Build(
                Messages.CheckIn_GetStarted,
                command.MessageContext.LanguageId);

            return CommandResult.Build(NotificationMessage.Create(command.MessageContext.Person.Id, messageText));
        }
        
        var existsMap = await _locationsRepository.Find(command.MessageContext.ChatMessage.ChatId, token);
        var map = existsMap ?? new(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            command.MessageContext.ChatMessage.ChatId,
            command.MessageContext.ChatName!);
        
        var location = new LocationOnMap(
            Guid.NewGuid(),
            command.MessageContext.Person.Id,
            command.MessageContext.Person.DisplayName,
            command.MessageContext.Location!.X,
            command.MessageContext.Location.Y,
            map);

        await _locationsRepository.Insert(location, token);

        if (existsMap is not null)
            return CommandResult.Empty;
        
        var message = await _messageBuilder.Build(
            Messages.CheckIn_ConnectLinkText,
            command.MessageContext.LanguageId,
            _mapLinksBuilder.Build(command.MessageContext.LanguageId, map.Id));

        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            message,
            pinned: true);
            
        return CommandResult.Build(notification);
    }
}