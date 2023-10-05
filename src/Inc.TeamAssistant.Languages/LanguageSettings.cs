using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Languages;

public static class LanguageSettings
{
    public static readonly LanguageId[] LanguageIds = { new("en"), new("ru") };
    public static readonly LanguageId DefaultLanguageId = LanguageIds.First();
    public static readonly int LanguageCodeLength = DefaultLanguageId.Value.Length;
}