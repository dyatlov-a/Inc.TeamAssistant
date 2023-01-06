using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.Appraiser.Dashboard.Components.Features.Annotations;

internal sealed record UserFriendlyAnnotation
{
    public MarkupString ResponsiveInterface { get; }
    public MarkupString UserActionsDisplayedInTelegram { get; }
    public MarkupString UseHelp { get; }

    public UserFriendlyAnnotation(
        string responsiveInterface,
        string userActionsDisplayedInTelegram,
        string useHelp)
    {
        ResponsiveInterface = (MarkupString)responsiveInterface;
        UserActionsDisplayedInTelegram = (MarkupString)userActionsDisplayedInTelegram;
        UseHelp = (MarkupString)useHelp;
    }

    public static readonly UserFriendlyAnnotation Empty = new(
        string.Empty,
        string.Empty,
        string.Empty);
}