using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Pages.Annotations;

internal sealed record MapAnnotation
{
    public MarkupString PageTitle { get; }
    public string DefaultLayerTitle { get; }

    public MapAnnotation(string pageTitle, string defaultLayerTitle)
    {
        PageTitle = (MarkupString)pageTitle;
        DefaultLayerTitle = defaultLayerTitle;
    }

    public static readonly MapAnnotation Empty = new(string.Empty, string.Empty);
}