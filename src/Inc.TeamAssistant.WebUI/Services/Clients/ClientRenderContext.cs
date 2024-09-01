using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class ClientRenderContext : IRenderContext
{
    private readonly NavigationManager _navigationManager;

    public ClientRenderContext(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
    }

    public bool IsBrowser => true;

    public bool IsDevelopment() => _navigationManager.BaseUri.StartsWith(
        "http://localhost",
        StringComparison.InvariantCultureIgnoreCase);

    public (LanguageId Language, bool Selected) GetCurrentLanguageId()
    {
        var relativeUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

        if (!string.IsNullOrWhiteSpace(relativeUrl))
        {
            var languageId = LanguageSettings.LanguageIds.SingleOrDefault(l => relativeUrl.StartsWith(
                l.Value,
                StringComparison.InvariantCultureIgnoreCase));

            if (languageId is not null)
                return (languageId, true);
        }
        
        return (LanguageSettings.DefaultLanguageId, false);
    }
}