using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Parsers;

internal sealed record TelegramMessageAdapter(string BotName, string Text, IReadOnlyCollection<long> PersonIds)
    : IInputMessage
{
    public static TelegramMessageAdapter Create(string botName, Message message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(botName);
        ArgumentNullException.ThrowIfNull(message);

        var personIds = message.Entities
            ?.Where(e => e is { Type: MessageEntityType.TextMention, User: not null })
            .Select(e => e.User!.Id)
            .ToArray();

        return new TelegramMessageAdapter(botName, message.Text ?? string.Empty, personIds ?? []);
    }
}