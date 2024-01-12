using Inc.TeamAssistant.Primitives;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class MessageExtensions
{
    public static UserIdentity? GetTargetUser(this Message message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        
        const char usernameMarker = '@';
        var username = message.Text!.Split(usernameMarker).LastOrDefault()?.Trim();
        var targetUserIds = message.Entities
            ?.Where(e => e is { Type: MessageEntityType.TextMention, User: not null })
            .Select(e => (e.User!.Id, e.User.FirstName))
            .ToArray();
        
        return targetUserIds?.Any() == true
            ? UserIdentity.Create(targetUserIds.Last().Id)
            : string.IsNullOrWhiteSpace(username)
                ? null
                : UserIdentity.Create(username);
    }
}