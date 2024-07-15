using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.WebUI.Features.Common;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Shared;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed record ReviewTotalStatsWidgetViewModel(
    string ReviewByReviewer,
    string ReviewByOwner,
    IReadOnlyCollection<DateSelectorItem> DateItems,
    IReadOnlyCollection<HistoryByTeamItemDto> Review,
    IReadOnlyCollection<HistoryByTeamItemDto> Requests)
    : IViewModel<ReviewTotalStatsWidgetViewModel>
{
    public static ReviewTotalStatsWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        Array.Empty<DateSelectorItem>(),
        Array.Empty<HistoryByTeamItemDto>(),
        Array.Empty<HistoryByTeamItemDto>());
}