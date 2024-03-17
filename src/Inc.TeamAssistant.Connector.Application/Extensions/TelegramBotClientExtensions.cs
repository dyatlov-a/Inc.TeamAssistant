using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class TelegramBotClientExtensions
{
    public static async Task TryDelete(
        this ITelegramBotClient client,
        IReadOnlyCollection<(long ChatId, int MessageId)> messages,
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
            logger.LogError(ex, "Can not delete messages");
        }
    }
    
    public static async Task TrySend(
        this ITelegramBotClient client,
        DialogState? dialog,
        long chatId,
        int messageId,
        string text,
        ILogger logger,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(logger);
        
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        try
        {
            var message = await client.SendTextMessageAsync(chatId, text, cancellationToken: token);
            
            if (dialog is not null)
            {
                dialog.Attach(new ChatMessage(chatId, messageId));
                dialog.Attach(new ChatMessage(chatId, message.MessageId));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can not send message to chat {TargetChatId}", chatId);
        }
    }
}