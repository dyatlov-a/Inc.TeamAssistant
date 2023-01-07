using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Components.Annotations;

internal sealed record DevelopmentAnnotation
{
    public MarkupString UseModernTechnologies { get; }
    public MarkupString ReuseCode { get; }
    public MarkupString OpenSource { get; }

    public DevelopmentAnnotation(string useModernTechnologies, string reuseCode, string openSource)
    {
        UseModernTechnologies = (MarkupString)useModernTechnologies;
        ReuseCode = (MarkupString)reuseCode;
        OpenSource = (MarkupString)openSource;
    }

    public static readonly DevelopmentAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty);
}