using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Reviewer.All.Extensions;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Reviewer.All.Services;

public sealed record CommandContext(
    int? MessageId,
    long UserId,
    string UserName,
    string? UserLogin,
    LanguageId LanguageId,
    long ChatId,
    string Text)
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
            update.Message.From.Id,
            update.Message.From.GetUserName(),
            update.Message.From.Username,
            update.Message.From.GetLanguageId(),
            update.Message.Chat.Id,
            commandText);
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
            update.CallbackQuery.From.Id,
            update.CallbackQuery.From.GetUserName(),
            update.CallbackQuery.From.Username,
            update.CallbackQuery.From.GetLanguageId(),
            update.CallbackQuery.Message.Chat.Id,
            commandText);
    }
}