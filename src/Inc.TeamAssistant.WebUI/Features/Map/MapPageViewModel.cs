namespace Inc.TeamAssistant.WebUI.Features.Map;

public sealed record MapPageViewModel(string DefaultLayerTitle, string ShowRouteText, string HideRouteText)
{
    public static readonly MapPageViewModel Empty = new(string.Empty, string.Empty, string.Empty);
}