using Inc.TeamAssistant.Primitives;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class NotificationMessageExtensions
{
    public static InlineKeyboardMarkup? ToReplyMarkup(this NotificationMessage message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        
        const int rowCapacity = 5;
		
        return message.Buttons.Any()
            ? new InlineKeyboardMarkup(message.Buttons
                .Select(b => InlineKeyboardButton.WithCallbackData(b.Text, b.Data))
                .Chunk(rowCapacity))
            : null;
    }
}