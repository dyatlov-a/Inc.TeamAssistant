namespace Inc.TeamAssistant.WebUI.Features.Common;

public sealed record MainNavbarViewModel(
    bool HasLoginAsSuperUser,
    string MainPageLink,
    string Logout,
    string LoginAsSuperuser)
    : IViewModel<MainNavbarViewModel>
{
    public static MainNavbarViewModel Empty { get; } = new(
        false,
        string.Empty,
        string.Empty,
        string.Empty);
}