using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap;

internal sealed class AddLocationToMapCommandHandler : IRequestHandler<AddLocationToMapCommand, CommandResult>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly CheckInOptions _options;
    private readonly ITranslateProvider _translateProvider;

    public AddLocationToMapCommandHandler(
        ILocationsRepository locationsRepository,
        CheckInOptions options,
        ITranslateProvider translateProvider)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<CommandResult> Handle(AddLocationToMapCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (!command.MessageContext.Location.HasValue)
            return CommandResult.Empty;
        
        if (command.MessageContext.ChatId == command.MessageContext.PersonId)
        {
            var messageText = await _translateProvider.Get(
                Messages.CheckIn_GetStarted,
                command.MessageContext.LanguageId);

            return CommandResult.Build(NotificationMessage.Create(command.MessageContext.PersonId, messageText));
        }
        
        var existsMap = await _locationsRepository.Find(command.MessageContext.ChatId, token);
        var map = existsMap ?? new(command.MessageContext.ChatId);
        
        var location = new LocationOnMap(
            command.MessageContext.PersonId,
            command.MessageContext.DisplayUsername,
            command.MessageContext.Location.Value.Longitude,
            command.MessageContext.Location.Value.Latitude,
            map);

        await _locationsRepository.Insert(location, token);

        var notifications = new List<NotificationMessage>
        {
            NotificationMessage.Delete(new ChatMessage(command.MessageContext.ChatId, command.MessageContext.MessageId))
        };

        if (existsMap is null)
        {
            var link = string.Format(
                _options.ConnectToMapLinkTemplate,
                command.MessageContext.LanguageId.Value,
                map.Id.ToString("N"));
            var message = await _translateProvider.Get(
                Messages.CheckIn_ConnectLinkText,
                command.MessageContext.LanguageId,
                link);
            
            notifications.Add(NotificationMessage.Create(command.MessageContext.ChatId, message, pinned: true));
        }

        return CommandResult.Build(notifications.ToArray());
    }
}