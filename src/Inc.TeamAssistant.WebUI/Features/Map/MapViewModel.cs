using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Features.Map;

internal sealed record MapViewModel
{
    public MarkupString PageTitle { get; }
    public string DefaultLayerTitle { get; }

    public MapViewModel(string pageTitle, string defaultLayerTitle)
    {
        PageTitle = (MarkupString)pageTitle;
        DefaultLayerTitle = defaultLayerTitle;
    }

    public static readonly MapViewModel Empty = new(string.Empty, string.Empty);
}