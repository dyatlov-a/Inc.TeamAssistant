using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Dashboard;

public static class Settings
{
    public static readonly LanguageId[] LanguageIds = { new("en"), new("ru") };
    public static readonly LanguageId DefaultLanguageId = LanguageIds.First();
    public static readonly int LanguageCodeLength = DefaultLanguageId.Value.Length;
    public static readonly string AnonymousUser = "Anonymous";
    public static readonly string FeedbackTo = "dyatlovall@gmail.com";
}