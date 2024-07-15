using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Teams;

public sealed record TeammatesWidgetViewModel(
    bool HasManagerAccess,
    string PersonTitle,
    string LeaveUntilTitle,
    string ExcludeFromTeamTitle,
    string LeaveTeammate,
    string Days,
    string Forever,
    string RecoveryTeammate,
    IReadOnlyCollection<TeammateDto> Teammates)
    : IViewModel<TeammatesWidgetViewModel>
{
    public static TeammatesWidgetViewModel Empty { get; } = new(
        false,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<TeammateDto>());
}