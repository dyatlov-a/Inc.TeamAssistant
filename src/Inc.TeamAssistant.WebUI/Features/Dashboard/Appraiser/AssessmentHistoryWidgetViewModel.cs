using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public record AssessmentHistoryWidgetViewModel(
    string TasksName,
    string GoToCurrentSessionButtonText,
    string AssessmentDate,
    IReadOnlyCollection<DateSelectorItem> DateItems,
    IReadOnlyCollection<AssessmentHistoryDto> Items)
    : IViewModel<AssessmentHistoryWidgetViewModel>
{
    public static AssessmentHistoryWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<DateSelectorItem>(),
        Array.Empty<AssessmentHistoryDto>());
}