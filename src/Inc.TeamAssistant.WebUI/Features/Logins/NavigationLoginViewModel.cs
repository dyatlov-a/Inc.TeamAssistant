using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Logins;

public sealed record NavigationLoginViewModel(bool HasLoginAsSuperUser, string Logout, string LoginAsSuperuser)
    : IViewModel<NavigationLoginViewModel>
{
    public static NavigationLoginViewModel Empty { get; } = new(false, string.Empty, string.Empty);
}