using Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;
using Inc.TeamAssistant.Reviewer.All.Extensions;
using Inc.TeamAssistant.Reviewer.All.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Reviewer.All.Services;

public sealed record CommandContext
{
    private int? MessageId { get; init; }
    public long ChatId { get; private init; }
    public string Text { get; private init; } = default!;
    public Person Person { get; private init; } = default!;
    public UserIdentity? TargetUser  { get; private init; }

    private CommandContext()
    {
    }
    
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

        const char usernameMarker = '@';
        var commandText = update.Message.Text
            .Replace($"@{botName}", string.Empty, StringComparison.InvariantCultureIgnoreCase)
            .Trim();
        var targetUserIds = update.Message.Entities
            ?.Where(e => e is { Type: MessageEntityType.TextMention, User: { } })
            .Select(e => (e.User!.Id, e.User.FirstName))
            .ToArray();
        var username = commandText.Split(usernameMarker).LastOrDefault()?.Trim();
        var cleanCommandText = targetUserIds?.Any() == true
            ? commandText.Replace(targetUserIds.Last().FirstName, string.Empty)
            : !string.IsNullOrWhiteSpace(username)
                ? commandText.Replace($"{usernameMarker}{username}", string.Empty)
                : commandText;
        
        if (string.IsNullOrWhiteSpace(cleanCommandText))
            return null;
        
        var userIdentity = targetUserIds?.Any() == true
            ? UserIdentity.Create(targetUserIds.Last().Id)
            : string.IsNullOrWhiteSpace(username)
                ? null
                : UserIdentity.Create(username);

        return new CommandContext
        {
            MessageId = update.Message.MessageId,
            ChatId = update.Message.Chat.Id,
            Text = cleanCommandText,
            Person = new Person(
                update.Message.From.Id,
                update.Message.From.GetLanguageId(),
                update.Message.From.FirstName,
                update.Message.From.LastName,
                update.Message.From.Username),
            TargetUser = userIdentity
        };
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
        
        return new CommandContext
        {
            ChatId = update.CallbackQuery.Message.Chat.Id,
            Text = commandText,
            Person = new Person(
                update.CallbackQuery.From.Id,
                update.CallbackQuery.From.GetLanguageId(),
                update.CallbackQuery.From.FirstName,
                update.CallbackQuery.From.LastName,
                update.CallbackQuery.From.Username)
        };
    }

    public ChatMessage? ToChatMessage() => MessageId.HasValue ? new ChatMessage(ChatId, MessageId.Value) : null;

    public bool IsPrivate() => ChatId == Person.Id;
}