using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class ClientRenderContext(NavigationManager navigationManager) : IRenderContext
{
    public bool IsClientSide => true;

    public LanguageId? SelectedLanguage => LanguageSettings.LanguageIds
        .SingleOrDefault(l => navigationManager.ToBaseRelativePath(navigationManager.Uri).StartsWith(
            l.Value,
            StringComparison.InvariantCultureIgnoreCase));
}