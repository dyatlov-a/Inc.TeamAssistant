using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Pages.Annotations;

internal sealed record MainPageAnnotation
{
    public MarkupString MainTitle { get; }
    public MarkupString MainDescription { get; }

    public MainPageAnnotation(string mainTitle, string mainDescription)
    {
        MainTitle = (MarkupString)mainTitle;
        MainDescription = (MarkupString)mainDescription;
    }
    
    public static readonly MainPageAnnotation Empty = new(string.Empty, string.Empty);
}