namespace Inc.TeamAssistant.WebUI.Features.Common;

public sealed record MainNavbarViewModel(
    bool HasLoginControls,
    string CurrentLanguage,
    string Login,
    string Logout)
    : IViewModel<MainNavbarViewModel>
{
    public static MainNavbarViewModel Empty { get; } = new(
        false,
        string.Empty,
        string.Empty,
        string.Empty);
}