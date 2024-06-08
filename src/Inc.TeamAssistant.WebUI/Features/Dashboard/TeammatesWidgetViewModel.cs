using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record TeammatesWidgetViewModel(IReadOnlyCollection<TeammateDto> Teammates)
    : IViewModel<TeammatesWidgetViewModel>
{
    public static TeammatesWidgetViewModel Empty { get; } = new(Array.Empty<TeammateDto>());
}