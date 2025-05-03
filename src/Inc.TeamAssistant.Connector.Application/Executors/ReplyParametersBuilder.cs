using Inc.TeamAssistant.Primitives.Notifications;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Executors;

internal static class ReplyParametersBuilder
{
    public static ReplyParameters? Build(NotificationMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        
        return message.ReplyToMessageId.HasValue
            ? new ReplyParameters
            {
                MessageId = message.ReplyToMessageId.Value
            }
            : null;
    }
}