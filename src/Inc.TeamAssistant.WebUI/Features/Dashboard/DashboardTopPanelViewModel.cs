using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record DashboardTopPanelViewModel(bool HasLoginAsSuperUser, string Logout, string LoginAsSuperuser)
    : IViewModel<DashboardTopPanelViewModel>
{
    public static DashboardTopPanelViewModel Empty { get; } = new(false, string.Empty, string.Empty);
}