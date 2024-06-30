using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public record SessionHistoryWidgetViewModel(
    string TasksName,
    IReadOnlyCollection<AssessmentHistoryDto> HistoryItems,
    StoryDto? CurrentStory,
    string GoToCurrentSessionButtonText,
    string AssessmentDate) : IViewModel<SessionHistoryWidgetViewModel>
{
    public static SessionHistoryWidgetViewModel Empty { get; } = new(
        string.Empty,
        Array.Empty<AssessmentHistoryDto>(), 
        null,
        string.Empty,
        string.Empty);
}