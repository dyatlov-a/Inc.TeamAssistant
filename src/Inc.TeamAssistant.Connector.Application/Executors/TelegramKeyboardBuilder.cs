using Inc.TeamAssistant.Primitives.Notifications;
using Telegram.Bot.Types.ReplyMarkups;

namespace Inc.TeamAssistant.Connector.Application.Executors;

internal static class TelegramKeyboardBuilder
{
    public static InlineKeyboardMarkup Build(NotificationMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message.Buttons.Any())
        {
            var buttons = message.Buttons
                .Select(b => InlineKeyboardButton.WithCallbackData(b.Text, b.Data))
                .Chunk(message.ButtonsInRow);

            return new InlineKeyboardMarkup(buttons);
        }
        
        return new InlineKeyboardMarkup(Array.Empty<InlineKeyboardButton>());
    }
}