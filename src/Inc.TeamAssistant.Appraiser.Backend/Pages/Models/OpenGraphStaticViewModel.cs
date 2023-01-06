using Inc.TeamAssistant.Appraiser.Dashboard;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Backend.Pages.Models;

public sealed record OpenGraphStaticViewModel(LanguageId LanguageId, string Title, string Description, string ImageName)
{
    public string GetLocale() => $"{LanguageId.Value.ToLower()}_{LanguageId.Value.ToUpper()}";

    public static readonly OpenGraphStaticViewModel Empty = new(
        Settings.DefaultLanguageId,
        string.Empty,
        string.Empty,
        string.Empty);
}