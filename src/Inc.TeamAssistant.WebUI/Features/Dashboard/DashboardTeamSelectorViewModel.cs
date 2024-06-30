using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record DashboardTeamSelectorViewModel(
    string SelectTeamTitle,
    string SelectTeam,
    string TeamField,
    string BotField,
    string ConnectToTeam)
    : IViewModel<DashboardTeamSelectorViewModel>
{
    public static DashboardTeamSelectorViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}