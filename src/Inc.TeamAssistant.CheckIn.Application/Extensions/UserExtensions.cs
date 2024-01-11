using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.CheckIn.Application.Extensions;

public static class UserExtensions
{
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