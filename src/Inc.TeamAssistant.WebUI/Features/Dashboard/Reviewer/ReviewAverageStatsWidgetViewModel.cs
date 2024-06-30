using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed record ReviewAverageStatsWidgetViewModel(
    string Title,
    string FirstTouch,
    string Review,
    string Correction,
    IReadOnlyCollection<DateSelectorItem> DateItems,
    IReadOnlyCollection<ReviewAverageStatsDto> Items)
    : IViewModel<ReviewAverageStatsWidgetViewModel>
{
    public static ReviewAverageStatsWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<DateSelectorItem>(),
        Array.Empty<ReviewAverageStatsDto>());
}