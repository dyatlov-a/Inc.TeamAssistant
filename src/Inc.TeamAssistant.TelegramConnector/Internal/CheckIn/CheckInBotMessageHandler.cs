using System.Text.Json;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.TelegramConnector.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.TelegramConnector.Internal.CheckIn;

internal sealed class CheckInBotMessageHandler : IMessageHandler
{
    private readonly ILogger<CheckInBotMessageHandler> _logger;
    private readonly ICheckInService _checkInService;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _connectToMapLinkTemplate;

    public CheckInBotMessageHandler(
        ILogger<CheckInBotMessageHandler> logger,
        ICheckInService checkInService,
        IServiceProvider serviceProvider,
        string connectToMapLinkTemplate)
    {
        if (string.IsNullOrWhiteSpace(connectToMapLinkTemplate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectToMapLinkTemplate));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _checkInService = checkInService ?? throw new ArgumentNullException(nameof(checkInService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _connectToMapLinkTemplate = connectToMapLinkTemplate;
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        try
        {
            if (update.Message?.From is null || update.Message.From.IsBot)
                return;

            using var scope = _serviceProvider.CreateScope();
            var messageBuilder = scope.ServiceProvider.GetRequiredService<IMessageBuilder>();
            var languageId = update.Message.From.GetLanguageId();

            if (update.Message.Chat.Id == update.Message.From.Id)
            {
                var messageText = await messageBuilder.Build(Messages.CheckIn_GetStarted, languageId);
                await client.SendTextMessageAsync(
                    update.Message.From.Id,
                    messageText,
                    cancellationToken: cancellationToken);

                return;
            }

            if (update.Message?.Location is null)
                return;

            var source = JsonSerializer.Serialize(update);
            var existsMap = await _checkInService.AddLocationToMap(new AddLocationToMapCommand(
                    update.Message.Chat.Id,
                    source,
                    update.Message.From.Id,
                    update.Message.From.GetUserName(),
                    update.Message.Location.Longitude,
                    update.Message.Location.Latitude),
                cancellationToken);

            await client.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId, cancellationToken);

            if (existsMap.Result.IsCreated)
            {
                var link = string.Format(
                    _connectToMapLinkTemplate,
                    languageId.Value,
                    existsMap.Result.MapId.Value.ToString("N"));

                var linkToMap = await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    await messageBuilder.Build(Messages.CheckIn_ConnectLinkText, languageId, link),
                    cancellationToken: cancellationToken);

                await client.PinChatMessageAsync(
                    update.Message.Chat.Id,
                    linkToMap.MessageId,
                    cancellationToken: cancellationToken);
            }
        }
        catch (ApiRequestException ex)
        {
            _logger.LogWarning(ex, "Error from telegram API");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
    }

    public Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message listened with error");

        return Task.CompletedTask;
    }
}