using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Shared.Annotations;

internal sealed record MainMenuAnnotation
{
    public MarkupString Appraiser { get; }
    public MarkupString MoveToBot { get; }
    public MarkupString Logo { get; }

    public MainMenuAnnotation(string appraiser, string moveToBot, string logo)
    {
        Appraiser = (MarkupString)appraiser;
        MoveToBot = (MarkupString)moveToBot;
        Logo = (MarkupString)logo;
    }

    public static readonly MainMenuAnnotation Empty = new(string.Empty, string.Empty, string.Empty);
}