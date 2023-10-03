using Inc.TeamAssistant.WebUI.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Pages.ViewModels;

internal sealed record ProductViewModel
{
    public MarkupString Description { get; }
    public MarkupString LinkText { get; }
    public string LinkUrl { get; }
    public ImageViewModel Image { get; }
    public string? PageUrl { get; }

    public ProductViewModel(
        string description,
        string linkText,
        string linkUrl,
        ImageViewModel image,
        string? pageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));
        if (string.IsNullOrWhiteSpace(linkText))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkText));
        if (string.IsNullOrWhiteSpace(linkUrl))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkUrl));
        
        Description = (MarkupString)description;
        LinkText = (MarkupString)linkText;
        LinkUrl = linkUrl;
        Image = image ?? throw new ArgumentNullException(nameof(image));
        PageUrl = string.IsNullOrWhiteSpace(pageUrl) ? null : pageUrl.Trim();
    }
}