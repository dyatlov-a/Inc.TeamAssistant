using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Pages.Annotations;

internal sealed record MainPageAnnotation
{
    public MarkupString Features { get; }
    public MarkupString Tools { get; }
    public MarkupString Feedback { get; }

    public MainPageAnnotation(string advantages, string tools, string feedback)
    {
        Features = (MarkupString)advantages;
        Tools = (MarkupString)tools;
        Feedback = (MarkupString)feedback;
    }

    public static readonly MainPageAnnotation Empty = new(string.Empty, string.Empty, string.Empty);
}