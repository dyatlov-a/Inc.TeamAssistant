namespace Inc.TeamAssistant.WebUI.Features.Map;

public sealed record MapViewModel
{
    public string DefaultLayerTitle { get; }
    public string ShowRouteText { get; }
    public string HideRouteText { get; }
    
    public MapViewModel(string defaultLayerTitle, string showRouteText, string hideRouteText)
    {
        DefaultLayerTitle = defaultLayerTitle;
        ShowRouteText = showRouteText;
        HideRouteText = hideRouteText;
    }
    
    public static readonly MapViewModel Empty = new(string.Empty, string.Empty, string.Empty);
}