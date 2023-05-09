using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Pages.Annotations;

internal sealed record AppraiserPageAnnotation
{
    public MarkupString Features { get; }
    public MarkupString Tools { get; }
    public MarkupString Feedback { get; }

    public AppraiserPageAnnotation(string advantages, string tools, string feedback)
    {
        Features = (MarkupString)advantages;
        Tools = (MarkupString)tools;
        Feedback = (MarkupString)feedback;
    }

    public static readonly AppraiserPageAnnotation Empty = new(string.Empty, string.Empty, string.Empty);
}