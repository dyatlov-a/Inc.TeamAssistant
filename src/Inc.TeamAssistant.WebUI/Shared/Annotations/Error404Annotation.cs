using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Shared.Annotations;

internal sealed record Error404Annotation
{
    public MarkupString Title { get; }
    public MarkupString Description { get; }

    public Error404Annotation(string title, string description)
    {
        Title = (MarkupString)title;
        Description = (MarkupString)description;
    }

    public static readonly Error404Annotation Empty = new(string.Empty, string.Empty);
}