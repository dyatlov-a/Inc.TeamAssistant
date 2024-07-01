namespace Inc.TeamAssistant.WebUI.Features.Common;

public sealed record MainNavbarViewModel(
    bool HasLoginControls,
    string MainUrl,
    string LoginUrl,
    string LoginText,
    string LogoutUrl,
    string LogoutText)
    : IViewModel<MainNavbarViewModel>
{
    public static MainNavbarViewModel Empty { get; } = new(
        false,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}