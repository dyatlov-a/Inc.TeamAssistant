using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.ViewModels;

internal sealed record ImageViewModel
{
    public string Path { get; }
    public MarkupString Alt { get; }

    public ImageViewModel(string path, string alt)
    {
        Path = path;
        Alt = (MarkupString)alt;
    }
}