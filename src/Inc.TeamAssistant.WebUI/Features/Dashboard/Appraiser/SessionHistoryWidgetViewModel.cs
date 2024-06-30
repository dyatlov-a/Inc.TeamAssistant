using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public record SessionHistoryWidgetViewModel(
    string TasksName,
    string GoToCurrentSessionButtonText,
    string AssessmentDate,
    IReadOnlyCollection<AssessmentHistoryDto> Items)
    : IViewModel<SessionHistoryWidgetViewModel>
{
    public static SessionHistoryWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<AssessmentHistoryDto>());
}