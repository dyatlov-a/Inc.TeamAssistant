using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class NavRouter : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly NavigationManager _navigationManager;
    private Action? _onRouteChanged;

    public string CurrentUrl { get; private set; }

    public NavRouter(IRenderContext renderContext, NavigationManager navigationManager)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _navigationManager.LocationChanged += OnLocationChanged;
        CurrentUrl = _navigationManager.Uri;
    }
    
    public string GetRouteWithoutLanguage()
    {
        var routeWithoutLanguage = _navigationManager.ToBaseRelativePath(CurrentUrl);
        
        foreach (var languageId in LanguageSettings.LanguageIds)
            routeWithoutLanguage = routeWithoutLanguage.Replace($"{languageId.Value}/", string.Empty);

        return routeWithoutLanguage;
    }

    public string CreateRoute(string? relativeUrl)
    {
        var link = string.IsNullOrWhiteSpace(relativeUrl) ? "/" : $"/{relativeUrl}";
        var languageContext = _renderContext.GetLanguageContext();

        return languageContext.Selected
            ? $"/{languageContext.CurrentLanguage.Value}{link}"
            : link;
    }

    public void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        
        _navigationManager.NavigateTo(uri, forceLoad, replace);
    }
    
    public void NavigateToRoute(string uri, bool forceLoad = false, bool replace = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        
        _navigationManager.NavigateTo(CreateRoute(uri), forceLoad, replace);
    }
    
    public IDisposable OnRouteChanged(Action onRouteChanged)
    {
        _onRouteChanged += onRouteChanged;

        return new RouterScope(() => _onRouteChanged -= onRouteChanged);
    }
    
    public void ChangeRoute(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        ChangeRouteCore(_navigationManager.ToAbsoluteUri(uri).ToString());
    }
    
    private void ChangeRouteCore(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        CurrentUrl = uri;
        _onRouteChanged?.Invoke();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => ChangeRouteCore(e.Location);

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}