using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Pages.Models;

public sealed record OpenGraphStaticViewModel(LanguageId LanguageId, string Title, string Description, string ImageName)
{
    public string GetLocale() => $"{LanguageId.Value.ToLower()}_{LanguageId.Value.ToUpper()}";

    public static readonly OpenGraphStaticViewModel Empty = new(
        LanguageSettings.DefaultLanguageId,
        string.Empty,
        string.Empty,
        string.Empty);
}