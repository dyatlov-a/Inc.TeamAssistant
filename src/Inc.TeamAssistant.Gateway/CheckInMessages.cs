using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Gateway;

internal static class CheckInMessages
{
    public static readonly MessageId CheckIn_OgTitle = new(nameof(CheckIn_OgTitle));
    public static readonly MessageId CheckIn_OgDescription = new(nameof(CheckIn_OgDescription));
}