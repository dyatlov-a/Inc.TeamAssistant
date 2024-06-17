using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed record ReviewTotalStatsWidgetViewModel(
    string ReviewByReviewer,
    string ReviewByOwner,
    IReadOnlyCollection<HistoryByTeamItemDto> Review,
    IReadOnlyCollection<HistoryByTeamItemDto> Requests)
    : IViewModel<ReviewTotalStatsWidgetViewModel>
{
    public static ReviewTotalStatsWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        Array.Empty<HistoryByTeamItemDto>(),
        Array.Empty<HistoryByTeamItemDto>());
}