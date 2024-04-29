using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record OpenGraphViewModel
{
    public MarkupString Title { get; }
    public MarkupString Description { get; }
    public string ImageName { get; }
    
    public OpenGraphViewModel(
        string title,
        string description,
        string imageName)
    {
        Title = (MarkupString)title;
        Description = (MarkupString)description;
        ImageName = imageName;
    }
    
    public static readonly OpenGraphViewModel Empty = new(string.Empty, string.Empty, string.Empty);
}