using Inc.TeamAssistant.Primitives.Notifications;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Executors;

internal static class TelegramEntityBuilder
{
    public static IReadOnlyCollection<MessageEntity> Build(NotificationMessage notificationMessage)
    {
        ArgumentNullException.ThrowIfNull(notificationMessage);

        return BuildCore(notificationMessage).ToArray();
    }
    
    private static IEnumerable<MessageEntity> BuildCore(NotificationMessage notificationMessage)
    {
        foreach (var target in notificationMessage.TargetPersons)
            yield return new MessageEntity
            {
                Type = MessageEntityType.TextMention,
                Offset = target.Offset,
                Length = target.Person.Name.Length,
                User = new User
                {
                    Id = target.Person.Id,
                    LanguageCode = target.LanguageId.Value,
                    FirstName = target.Person.Name,
                    Username = target.Person.Username
                }
            };
    }
}