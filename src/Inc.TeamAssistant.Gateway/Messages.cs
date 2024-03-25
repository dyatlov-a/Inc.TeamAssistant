using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Gateway;

internal static class Messages
{
    public static readonly MessageId MetaTitle = new(nameof(MetaTitle));
    public static readonly MessageId MetaDescription = new(nameof(MetaDescription));
    public static readonly MessageId MetaKeywords = new(nameof(MetaKeywords));
    public static readonly MessageId MetaAuthor = new(nameof(MetaAuthor));

    public static readonly MessageId OgTitle = new(nameof(OgTitle));
    public static readonly MessageId OgDescription = new(nameof(OgDescription));
}