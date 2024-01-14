namespace Inc.TeamAssistant.Primitives;

public static class LanguageSettings
{
    public static readonly LanguageId[] LanguageIds = { new("en"), new("ru") };
    public static readonly LanguageId DefaultLanguageId = LanguageIds.First();
    public static readonly int LanguageCodeLength = LanguageIds.Max(l => l.Value.Length);
}