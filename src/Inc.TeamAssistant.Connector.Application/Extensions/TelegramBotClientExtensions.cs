using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Notifications;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class TelegramBotClientExtensions
{
    public static async Task TryPinChatMessage(
        this ITelegramBotClient client,
        Guid botId,
        ChatMessage message,
        ILogger logger,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            await client.PinChatMessageAsync(new(message.ChatId), message.MessageId, cancellationToken: token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Bot {BotId} has not rights for pin message {Message}", botId, message);
        }
    }

    public static async Task TryDeleteMessage(
        this ITelegramBotClient client,
        Guid botId,
        ChatMessage message,
        ILogger logger,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(message);
        
        try
        {
            await client.DeleteMessageAsync(new(message.ChatId), message.MessageId, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Bot {BotId} has not rights for delete message {Message}", botId, message);
        }
    }
    
    public static async Task TryDeleteMessages(
        this ITelegramBotClient client,
        Guid botId,
        IReadOnlyCollection<ChatMessage> messages,
        ILogger logger,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentNullException.ThrowIfNull(logger);
        
        try
        {
            foreach (var message in messages)
                await client.DeleteMessageAsync(message.ChatId, message.MessageId, cancellationToken: token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Bot {BotId} has not rights for delete messages", botId);
        }
    }
    
    public static async Task TrySend(
        this ITelegramBotClient client,
        Guid botId,
        DialogState? dialog,
        ChatMessage chatMessage,
        string text,
        ILogger logger,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(chatMessage);
        ArgumentNullException.ThrowIfNull(logger);
        
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        try
        {
            var message = await client.SendTextMessageAsync(chatMessage.ChatId, text, cancellationToken: token);
            
            if (dialog is not null)
            {
                dialog.Attach(chatMessage);
                dialog.Attach(chatMessage with { MessageId = message.MessageId });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Bot {BotId} can not send message to chat {TargetChatId}", botId, chatMessage.ChatId);
        }
    }
}