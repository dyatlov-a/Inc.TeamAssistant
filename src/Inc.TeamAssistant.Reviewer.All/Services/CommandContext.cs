using Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;
using Inc.TeamAssistant.Reviewer.All.Extensions;
using Inc.TeamAssistant.Reviewer.All.Model;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Reviewer.All.Services;

public sealed record CommandContext(
    int? MessageId,
    long ChatId,
    string Text,
    Person Person)
{
    public static CommandContext? TryCreateFromMessage(Update update, string botName)
    {
        if (update is null)
            throw new ArgumentNullException(nameof(update));
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        if (update.Message?.From is null
            || update.Message.From.IsBot
            || string.IsNullOrWhiteSpace(update.Message.Text))
            return null;

        var commandText = update.Message.Text
            .Replace($"@{botName}", string.Empty, StringComparison.InvariantCultureIgnoreCase)
            .Trim();
        
        return new CommandContext(
            update.Message.MessageId,
            update.Message.Chat.Id,
            commandText,
            new Person(
                update.Message.From.Id,
                update.Message.From.GetLanguageId(),
                update.Message.From.FirstName,
                update.Message.From.LastName,
                update.Message.From.Username));
    }

    public static CommandContext? TryCreateFromQuery(Update update, string botName)
    {
        if (update is null)
            throw new ArgumentNullException(nameof(update));
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        if (update.CallbackQuery?.From is null
            || update.CallbackQuery.From.IsBot
            || update.CallbackQuery.Message is null
            || string.IsNullOrWhiteSpace(update.CallbackQuery.Data))
            return null;

        var commandText = update.CallbackQuery.Data
            .Replace($"@{botName}", string.Empty, StringComparison.InvariantCultureIgnoreCase)
            .Trim();
        
        return new CommandContext(
            MessageId: null,
            update.CallbackQuery.Message.Chat.Id,
            commandText,
            new Person(
                update.CallbackQuery.From.Id,
                update.CallbackQuery.From.GetLanguageId(),
                update.CallbackQuery.From.FirstName,
                update.CallbackQuery.From.LastName,
                update.CallbackQuery.From.Username));
    }

    public ChatMessage? ToChatMessage() => MessageId.HasValue ? new ChatMessage(ChatId, MessageId.Value) : null;

    public bool IsPrivate() => ChatId == Person.Id;
}