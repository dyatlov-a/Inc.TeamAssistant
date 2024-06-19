using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed record ReviewerModuleViewModel(
    string ReviewTotalStatsWidgetTitle,
    string LastTasksWidgetTitle,
    string ReviewAverageStatsWidgetTitle)
    : IViewModel<ReviewerModuleViewModel>
{
    public static ReviewerModuleViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty);
}