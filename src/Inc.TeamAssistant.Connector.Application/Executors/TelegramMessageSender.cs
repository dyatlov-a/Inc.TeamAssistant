using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Executors;

internal sealed class TelegramMessageSender
{
    private readonly ILogger<TelegramMessageSender> _logger;

    public TelegramMessageSender(ILogger<TelegramMessageSender> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IContinuationCommand?> Send(
        ITelegramBotClient client,
        NotificationMessage notificationMessage,
        MessageContext messageContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(notificationMessage);
        ArgumentNullException.ThrowIfNull(messageContext);

        var entities = notificationMessage.TargetPersons.Any()
            ? TelegramEntityBuilder.Build(notificationMessage)
            : [];

        if (notificationMessage.TargetChatId.HasValue)
        {
            var message = notificationMessage.Options.Any()
                ? await client.SendPollAsync(
                    notificationMessage.TargetChatId.Value,
                    notificationMessage.Text,
                    notificationMessage.Options,
                    isAnonymous: false,
                    cancellationToken: token)
                : await client.SendTextMessageAsync(
                    notificationMessage.TargetChatId.Value,
                    notificationMessage.Text,
                    replyMarkup: TelegramKeyboardBuilder.Build(notificationMessage),
                    entities: entities,
                    replyToMessageId: notificationMessage.ReplyToMessageId,
                    cancellationToken: token);

            if (notificationMessage.Pinned)
                await TryPinChatMessage(
                    client,
                    new(notificationMessage.TargetChatId.Value, message.MessageId),
                    token);

            if (notificationMessage.Handler is not null)
            {
                var parameter = message.Poll?.Id ?? message.MessageId.ToString();
                return notificationMessage.Handler(messageContext, parameter);
            }
        }

        if (notificationMessage.TargetMessage is not null)
            await client.EditMessageTextAsync(
                new(notificationMessage.TargetMessage.ChatId),
                notificationMessage.TargetMessage.MessageId,
                notificationMessage.Text,
                replyMarkup: TelegramKeyboardBuilder.Build(notificationMessage),
                entities: entities,
                cancellationToken: token);
        
        if (notificationMessage.DeleteMessage is not null)
            await TryDeleteMessage(client, notificationMessage.DeleteMessage, token);

        return null;
    }
    
    private async Task TryPinChatMessage(ITelegramBotClient client, ChatMessage message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            await client.PinChatMessageAsync(new(message.ChatId), message.MessageId, cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Bot has not rights for pin message {Message}", message);
        }
    }
    
    private async Task TryDeleteMessage(ITelegramBotClient client, ChatMessage message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(message);
        
        try
        {
            await client.DeleteMessageAsync(new(message.ChatId), message.MessageId, token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Bot has not rights for delete message {Message}", message);
        }
    }
}