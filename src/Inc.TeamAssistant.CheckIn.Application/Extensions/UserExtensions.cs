using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.WebUI;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.CheckIn.Application.Extensions;

internal static class UserExtensions
{
    public static string GetUserName(this User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return !string.IsNullOrWhiteSpace(user.LastName)
            ? $"{user.FirstName} {user.LastName}"
            : user.FirstName;
    }

    public static LanguageId GetLanguageId(this User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        var languageId = Settings.LanguageIds.SingleOrDefault(l =>
                l.Value.Equals(user.LanguageCode, StringComparison.InvariantCultureIgnoreCase))
            ?? Settings.DefaultLanguageId;

        return languageId;
    }
}