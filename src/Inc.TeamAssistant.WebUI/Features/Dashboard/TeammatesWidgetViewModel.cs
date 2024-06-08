using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record TeammatesWidgetViewModel(
    string ExcludeFromTeam,
    IReadOnlyCollection<TeammateDto> Teammates)
    : IViewModel<TeammatesWidgetViewModel>
{
    public static TeammatesWidgetViewModel Empty { get; } = new(
        string.Empty,
        Array.Empty<TeammateDto>());
}