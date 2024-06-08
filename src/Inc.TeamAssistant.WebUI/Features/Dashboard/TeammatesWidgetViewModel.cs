using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record TeammatesWidgetViewModel(
    string ExcludeFromTeam,
    string LeaveTeammate,
    string Days,
    string Forever,
    IReadOnlyCollection<TeammateDto> Teammates)
    : IViewModel<TeammatesWidgetViewModel>
{
    public static TeammatesWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<TeammateDto>());
}