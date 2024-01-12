using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class UserExtensions
{
    public static LanguageId GetLanguageId(this User user)
    {
        var userLanguageId = string.IsNullOrWhiteSpace(user.LanguageCode)
            ? LanguageSettings.DefaultLanguageId
            : new LanguageId(user.LanguageCode);
        
        return LanguageSettings.LanguageIds.Contains(userLanguageId)
            ? userLanguageId
            : LanguageSettings.DefaultLanguageId;
    }
}