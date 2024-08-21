using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.WebUI.Features.Common;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public record AppraiserHistoryWidgetViewModel(
    string StoriesCountText,
    string GoToCurrentSessionButtonText,
    string AssessmentSumText,
    string AssessmentDate,
    IReadOnlyCollection<DateSelectorItem> DateItems,
    IReadOnlyCollection<AssessmentHistoryDto> Items)
    : IViewModel<AppraiserHistoryWidgetViewModel>
{
    public static AppraiserHistoryWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<DateSelectorItem>(),
        Array.Empty<AssessmentHistoryDto>());
}