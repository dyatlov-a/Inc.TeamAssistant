using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

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

        if (notificationMessage is { ChatId: not null, PollMessageId: null })
        {
            var message = notificationMessage.Options.Any()
                ? await client.SendPoll(
                    chatId: notificationMessage.ChatId.Value,
                    question: notificationMessage.Text,
                    options: notificationMessage.Options.Select(o => new InputPollOption(o)),
                    isAnonymous: false,
                    cancellationToken: token)
                : await client.SendMessage(
                    chatId: notificationMessage.ChatId.Value,
                    text: notificationMessage.Text,
                    replyMarkup: TelegramKeyboardBuilder.Build(notificationMessage),
                    entities: entities,
                    replyParameters: ReplyParametersBuilder.Build(notificationMessage),
                    cancellationToken: token);

            if (notificationMessage.Pinned)
                await TryPinChatMessage(
                    client,
                    new(notificationMessage.ChatId.Value, message.MessageId),
                    token);

            if (notificationMessage.Handler is not null)
                return notificationMessage.Handler(messageContext, message.MessageId, message.Poll?.Id);
        }

        if (notificationMessage.EditedMessage is not null)
            await client.EditMessageText(
                chatId: notificationMessage.EditedMessage.ChatId,
                messageId: notificationMessage.EditedMessage.MessageId,
                text: notificationMessage.Text,
                replyMarkup: TelegramKeyboardBuilder.Build(notificationMessage),
                entities: entities,
                cancellationToken: token);
        
        if (notificationMessage.DeletedMessage is not null)
            await TryDeleteMessage(client, notificationMessage.DeletedMessage, token);
        
        if (notificationMessage is { ChatId: not null, PollMessageId: not null })
            await client.StopPoll(
                chatId: notificationMessage.ChatId.Value,
                messageId: notificationMessage.PollMessageId.Value,
                cancellationToken: token);

        return null;
    }
    
    private async Task TryPinChatMessage(ITelegramBotClient client, ChatMessage message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            await client.PinChatMessage(
                chatId: message.ChatId,
                messageId: message.MessageId,
                cancellationToken: token);
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
            await client.DeleteMessage(
                chatId: message.ChatId,
                messageId: message.MessageId,
                cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Bot has not rights for delete message {Message}", message);
        }
    }
}