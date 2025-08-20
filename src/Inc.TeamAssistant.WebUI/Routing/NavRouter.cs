using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace Inc.TeamAssistant.WebUI.Routing;

public sealed class NavRouter : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly NavigationManager _navigationManager;
    private readonly IServiceProvider _serviceProvider;
    private Action? _onRouteChanged;

    public string CurrentUrl { get; private set; }

    public NavRouter(
        IRenderContext renderContext,
        NavigationManager navigationManager,
        IServiceProvider serviceProvider)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _navigationManager.LocationChanged += OnLocationChanged;
        CurrentUrl = _navigationManager.Uri;
    }
    
    public string GetRouteSegment()
    {
        var routeWithoutLanguage = _navigationManager.ToBaseRelativePath(CurrentUrl);
        
        foreach (var languageId in LanguageSettings.LanguageIds)
            routeWithoutLanguage = routeWithoutLanguage.Replace($"{languageId.Value}/", string.Empty);

        return routeWithoutLanguage;
    }

    public NavRoute CreateRoute(string? routeSegment)
    {
        var link = string.IsNullOrWhiteSpace(routeSegment) ? "/" : routeSegment;

        return new NavRoute(_renderContext.SelectedLanguage, link);
    }
    
    public IDisposable OnRouteChanged(Action onRouteChanged)
    {
        _onRouteChanged += onRouteChanged;

        return new RouterScope(() => _onRouteChanged -= onRouteChanged);
    }

    public async Task MoveToRoute(string routeSegment, RoutingType type = RoutingType.Client)
    {
        var route = CreateRoute(routeSegment);
        
        switch (type)
        {
            case RoutingType.Browser:
                await ChangeBrowserPath(route);
                break;
            case RoutingType.Server:
                _navigationManager.NavigateTo(routeSegment, forceLoad: true);
                break;
            case RoutingType.Client:
            default:
                _navigationManager.NavigateTo(route);
                break;
        }
    }

    private async Task ChangeBrowserPath(NavRoute route)
    {
        ArgumentNullException.ThrowIfNull(route);
        
        var jsRuntime = _serviceProvider.GetRequiredService<IJSRuntime>();
        var jsFunction = JsFunctions.ChangeUrl(
            route,
            r => ChangeCurrentUrl(_navigationManager.ToAbsoluteUri(r).ToString()));

        await jsRuntime.Execute(jsFunction);
    }
    
    private void ChangeCurrentUrl(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        CurrentUrl = uri;
        _onRouteChanged?.Invoke();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => ChangeCurrentUrl(e.Location);

    public void Dispose() => _navigationManager.LocationChanged -= OnLocationChanged;
}