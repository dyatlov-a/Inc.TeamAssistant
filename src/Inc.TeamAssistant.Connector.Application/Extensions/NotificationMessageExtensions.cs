using Inc.TeamAssistant.Primitives.Notifications;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class NotificationMessageExtensions
{
    public static InlineKeyboardMarkup ToReplyMarkup(this NotificationMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        return message.Buttons.Any()
            ? new InlineKeyboardMarkup(message.Buttons
                .Select(b => InlineKeyboardButton.WithCallbackData(b.Text, b.Data))
                .Chunk(message.ButtonsInRow))
            : new InlineKeyboardMarkup(Array.Empty<InlineKeyboardButton>());
    }
}