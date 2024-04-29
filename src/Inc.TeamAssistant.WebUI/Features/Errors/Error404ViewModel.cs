using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Features.Errors;

internal sealed record Error404ViewModel
{
    public MarkupString Title { get; }
    public MarkupString Description { get; }

    public Error404ViewModel(string title, string description)
    {
        Title = (MarkupString)title;
        Description = (MarkupString)description;
    }

    public static readonly Error404ViewModel Empty = new(string.Empty, string.Empty);
}