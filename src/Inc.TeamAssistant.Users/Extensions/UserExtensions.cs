using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Users.Extensions;

public static class UserExtensions
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

        var languageId = LanguageSettings.LanguageIds.SingleOrDefault(l => l.Value.Equals(
                user.LanguageCode,
                StringComparison.InvariantCultureIgnoreCase))
            ?? LanguageSettings.DefaultLanguageId;

        return languageId;
    }
}