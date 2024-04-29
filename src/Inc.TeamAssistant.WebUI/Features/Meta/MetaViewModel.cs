using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record MetaViewModel
{
    public MarkupString Title { get; }
    public MarkupString Description { get; }
    public MarkupString Keywords { get; }
    public MarkupString Author { get; }

    public MetaViewModel(string title, string description, string keywords, string author)
    {
        Title = (MarkupString)title;
        Description = (MarkupString)description;
        Keywords = (MarkupString)keywords;
        Author = (MarkupString)author;
    }
    
    public static readonly MetaViewModel Empty = new(string.Empty, string.Empty, string.Empty, string.Empty);
}