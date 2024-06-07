using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record DashboardNavbarViewModel(bool HasLoginAsSuperUser, string Logout, string LoginAsSuperuser)
    : IViewModel<DashboardNavbarViewModel>
{
    public static DashboardNavbarViewModel Empty { get; } = new(false, string.Empty, string.Empty);
}