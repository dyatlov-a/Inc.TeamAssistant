using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class TeamAssistantRouter : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly NavigationManager _navigationManager;
    private Action _onRouteChanged = () => {};

    public string CurrentRelativeUrl { get; private set; }

    public TeamAssistantRouter(IRenderContext renderContext, NavigationManager navigationManager)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _navigationManager.LocationChanged += OnLocationChanged;
        CurrentRelativeUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
    }
    
    public string GetRouteWithoutLanguage()
    {
        var routeWithoutLanguage = CurrentRelativeUrl;
        
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
    
    public void OnRouteChanged(Action onRouteChanged)
    {
        _onRouteChanged += onRouteChanged;
    }
    
    public void ChangeRoute(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        CurrentRelativeUrl = uri;
        _onRouteChanged();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var relativeUrl = _navigationManager.ToBaseRelativePath(e.Location);
        
        ChangeRoute(relativeUrl);
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}