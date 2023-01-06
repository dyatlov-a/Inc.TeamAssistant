using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Shared.ViewModels;

internal sealed record LinkViewModel
{
    public MarkupString Title { get; }
    public string Url { get; }
    public string? Target { get; }
    public bool Selected { get; }

    public LinkViewModel(MarkupString title, string url, string? target = null, bool selected = false)
    {
        Title = title;
        Url = url;
        Target = !string.IsNullOrWhiteSpace(target) ? target : null;
        Selected = selected;
    }

    public LinkViewModel(string title, string url, string? target = null, bool selected = false)
        : this((MarkupString)title, url, target, selected)
    {
    }
}