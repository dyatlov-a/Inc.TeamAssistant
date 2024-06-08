using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record DashboardPageViewModel(string Title, string SelectTeam, string CreateBot)
    : IViewModel<DashboardPageViewModel>
{
    public static DashboardPageViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty);
}