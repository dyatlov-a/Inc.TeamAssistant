using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Notifications;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class TelegramBotClientExtensions
{
    public static async Task TryPinChatMessage(
        this ITelegramBotClient client,
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
            logger.LogWarning(ex, "Bot has not rights for pin message {Message}", message);
        }
    }

    public static async Task TryDeleteMessage(
        this ITelegramBotClient client,
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
            logger.LogWarning(ex, "Bot has not rights for delete message {Message}", message);
        }
    }
    
    public static async Task TryDeleteMessages(
        this ITelegramBotClient client,
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
            logger.LogError(ex, "Bot has not rights for delete messages");
        }
    }
    
    public static async Task TrySend(
        this ITelegramBotClient client,
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
            logger.LogError(ex, "Bot can not send message to chat {TargetChatId}", chatMessage.ChatId);
        }
    }
}