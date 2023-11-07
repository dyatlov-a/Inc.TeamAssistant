using System.Text.Json;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Users.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.CheckIn.Application.Services;

internal sealed class TelegramBotMessageHandler
{
    private readonly ILogger<TelegramBotMessageHandler> _logger;
    private readonly string _connectToMapLinkTemplate;
    private readonly IServiceProvider _serviceProvider;

    public TelegramBotMessageHandler(
        ILogger<TelegramBotMessageHandler> logger,
        IServiceProvider serviceProvider,
        string connectToMapLinkTemplate)
    {
        if (string.IsNullOrWhiteSpace(connectToMapLinkTemplate))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectToMapLinkTemplate));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            var translateProvider = scope.ServiceProvider.GetRequiredService<ITranslateProvider>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var languageId = update.Message.From.GetLanguageId();

            if (update.Message.Chat.Id == update.Message.From.Id)
            {
                var messageText = await translateProvider.Get(Messages.CheckIn_GetStarted, languageId);
                await client.SendTextMessageAsync(
                    update.Message.From.Id,
                    messageText,
                    cancellationToken: cancellationToken);

                return;
            }

            if (update.Message?.Location is null)
                return;
            
            var addLocationToMapCommand = new AddLocationToMapCommand(
                update.Message.Chat.Id,
                update.Message.From.Id,
                update.Message.From.FirstName,
                update.Message.Location.Longitude,
                update.Message.Location.Latitude,
                JsonSerializer.Serialize(update));
            
            var addLocationToMapResult = await mediator.Send(addLocationToMapCommand, cancellationToken);
            
            if (addLocationToMapResult.FirstLocationOnMap)
            {
                var link = string.Format(
                    _connectToMapLinkTemplate,
                    languageId.Value,
                    addLocationToMapResult.MapId.ToString("N"));
                var linkToMap = await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    await translateProvider.Get(Messages.CheckIn_ConnectLinkText, languageId, link),
                    cancellationToken: cancellationToken);

                await client.PinChatMessageAsync(
                    update.Message.Chat.Id,
                    linkToMap.MessageId,
                    cancellationToken: cancellationToken);
            }
            
            await client.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId, cancellationToken);
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