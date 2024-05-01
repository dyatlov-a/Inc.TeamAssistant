namespace Inc.TeamAssistant.Primitives.Languages;

public static class LanguageSettings
{
    public static readonly LanguageId[] LanguageIds = [new("en"), new("ru")];
    public static readonly LanguageId DefaultLanguageId = LanguageIds.First();
}