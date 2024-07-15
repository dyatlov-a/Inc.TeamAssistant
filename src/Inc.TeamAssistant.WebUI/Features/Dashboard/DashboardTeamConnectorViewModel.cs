using Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record DashboardTeamConnectorViewModel(
    string ConnectToTeamHelp,
    string ConnectToTeamButton,
    GetTeamConnectorResult TeamConnector)
    : IViewModel<DashboardTeamConnectorViewModel>
{
    public static DashboardTeamConnectorViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        new GetTeamConnectorResult(string.Empty, string.Empty, string.Empty));
}