using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Primitives;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class ClientInfoClient : IClientInfoService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;

    public ClientInfoClient(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
    }

    public async Task<LanguageId> GetCurrentLanguageId()
        => GetLanguageIdFromUrlOrDefault() ?? await GetLanguageIdFromClientOrDefault() ?? Settings.DefaultLanguageId;

    public LanguageId? GetLanguageIdFromUrlOrDefault()
    {
        var relativeUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

        return !string.IsNullOrWhiteSpace(relativeUrl) && relativeUrl.Length >= Settings.LanguageCodeLength
            ? Settings.LanguageIds.SingleOrDefault(l => relativeUrl.StartsWith(l.Value, StringComparison.InvariantCultureIgnoreCase))
            : null;
    }

    private async Task<LanguageId?> GetLanguageIdFromClientOrDefault()
    {
        var clientLanguage = await _jsRuntime.InvokeAsync<string?>("browserJsFunctions.getLanguage");

        return !string.IsNullOrWhiteSpace(clientLanguage) && clientLanguage.Length >= Settings.LanguageCodeLength
            ? Settings.LanguageIds.SingleOrDefault(l => clientLanguage.StartsWith(l.Value, StringComparison.InvariantCultureIgnoreCase))
            : null;
    }
}