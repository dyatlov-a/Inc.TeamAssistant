using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class UpdateExtensions
{
    public static string GetUserName(this Update update)
    {
        ArgumentNullException.ThrowIfNull(update);
        
        return update.Type switch
        {
            UpdateType.Message or UpdateType.EditedMessage => update.Message?.From?.ToLogEntry() ?? string.Empty,
            UpdateType.CallbackQuery => update.CallbackQuery?.From.ToLogEntry() ?? string.Empty,
            UpdateType.PollAnswer => update.PollAnswer?.User.ToLogEntry() ?? string.Empty,
            _ => string.Empty
        };
    }

    private static string ToLogEntry(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return $"(Id: {user.Id}, FirstName: {user.FirstName}, Username: {user.Username})";
    }
}