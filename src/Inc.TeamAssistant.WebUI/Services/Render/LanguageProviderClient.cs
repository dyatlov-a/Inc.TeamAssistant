using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Primitives.Languages;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.Render;

internal sealed class LanguageProviderClient : ILanguageProvider
{
    private readonly NavigationManager _navigationManager;

    public LanguageProviderClient(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
    }

    public (LanguageId Language, bool Selected) GetCurrentLanguageId()
    {
        var relativeUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

        if (!string.IsNullOrWhiteSpace(relativeUrl))
        {
            var languageId = LanguageSettings.LanguageIds.SingleOrDefault(l => relativeUrl.StartsWith(
                $"/{l.Value}",
                StringComparison.InvariantCultureIgnoreCase));

            if (languageId is not null)
                return (languageId, true);
        }
        
        return (LanguageSettings.DefaultLanguageId, false);
    }
}