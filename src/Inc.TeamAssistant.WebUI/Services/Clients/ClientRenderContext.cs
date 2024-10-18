using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class ClientRenderContext : IRenderContext
{
    private readonly NavigationManager _navigationManager;
    private readonly IWebAssemblyHostEnvironment _hostEnvironment;

    public ClientRenderContext(NavigationManager navigationManager, IWebAssemblyHostEnvironment hostEnvironment)
    {
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    }

    public bool IsBrowser => true;

    public bool IsDevelopment() => _hostEnvironment.IsDevelopment();

    public (LanguageId CurrentLanguage, bool Selected) GetLanguageContext()
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