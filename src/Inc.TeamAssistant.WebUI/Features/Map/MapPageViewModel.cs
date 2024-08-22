using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Map;

public sealed record MapPageViewModel(
    string DefaultLayerTitle,
    string ShowRouteText,
    string HideRouteText,
    IReadOnlyDictionary<string, string> FeatureNamesLookup)
    : IViewModel<MapPageViewModel>
{
    public static MapPageViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        new Dictionary<string, string>());
}