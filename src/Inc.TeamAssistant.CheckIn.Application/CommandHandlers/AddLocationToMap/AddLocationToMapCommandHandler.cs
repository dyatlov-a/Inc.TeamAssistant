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
        ArgumentNullException.ThrowIfNull(command);

        if (command.MessageContext.Location is null)
            return CommandResult.Empty;
        
        if (command.MessageContext.ChatMessage.ChatId == command.MessageContext.Person.Id)
        {
            var messageText = await _translateProvider.Get(
                Messages.CheckIn_GetStarted,
                command.MessageContext.LanguageId);

            return CommandResult.Build(NotificationMessage.Create(command.MessageContext.Person.Id, messageText));
        }
        
        var existsMap = await _locationsRepository.Find(command.MessageContext.ChatMessage.ChatId, token);
        var map = existsMap ?? new(command.MessageContext.Bot.Id, command.MessageContext.ChatMessage.ChatId);
        
        var location = new LocationOnMap(
            command.MessageContext.Person.Id,
            command.MessageContext.Person.DisplayName,
            command.MessageContext.Location.X,
            command.MessageContext.Location.Y,
            map);

        await _locationsRepository.Insert(location, token);

        var removeMessageNotification = NotificationMessage.Delete(command.MessageContext.ChatMessage);

        if (existsMap is not null)
            return CommandResult.Build(removeMessageNotification);
        
        var link = string.Format(
            _options.ConnectToMapLinkTemplate,
            command.MessageContext.LanguageId.Value,
            map.Id.ToString("N"));
        var message = await _translateProvider.Get(
            Messages.CheckIn_ConnectLinkText,
            command.MessageContext.LanguageId,
            link);

        var notification = NotificationMessage.Create(
            command.MessageContext.ChatMessage.ChatId,
            message,
            pinned: true);
            
        return CommandResult.Build(removeMessageNotification, notification);
    }
}