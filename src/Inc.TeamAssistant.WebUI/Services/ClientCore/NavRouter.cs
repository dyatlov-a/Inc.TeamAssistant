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

    public NavRoute CreateRoute(string? routeSegment)
    {
        var link = string.IsNullOrWhiteSpace(routeSegment) ? "/" : routeSegment;
        var languageContext = _renderContext.GetLanguageContext();
        var selectedLanguage = languageContext.Selected ? languageContext.CurrentLanguage : null;

        return new NavRoute(selectedLanguage, link);
    }

    public void NavigateToPath(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        
        _navigationManager.NavigateTo(uri, forceLoad: true);
    }
    
    public void NavigateToRouteSegment(string routeSegment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(routeSegment);

        var route = CreateRoute(routeSegment);
        
        NavigateToRoute(route);
    }
    
    public void NavigateToRoute(NavRoute route)
    {
        ArgumentNullException.ThrowIfNull(route);
        
        _navigationManager.NavigateTo(route);
    }
    
    public IDisposable OnRouteChanged(Action onRouteChanged)
    {
        _onRouteChanged += onRouteChanged;

        return new RouterScope(() => _onRouteChanged -= onRouteChanged);
    }
    
    public void ChangeRoute(NavRoute route)
    {
        ArgumentNullException.ThrowIfNull(route);

        ChangeRouteCore(_navigationManager.ToAbsoluteUri(route).ToString());
    }
    
    private void ChangeRouteCore(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        CurrentUrl = uri;
        _onRouteChanged?.Invoke();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => ChangeRouteCore(e.Location);

    public void Dispose() => _navigationManager.LocationChanged -= OnLocationChanged;
}