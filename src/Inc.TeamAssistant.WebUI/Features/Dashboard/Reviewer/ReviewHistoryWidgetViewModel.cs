using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using Inc.TeamAssistant.WebUI.Features.Common;
using Inc.TeamAssistant.WebUI.Features.Dashboard.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed record ReviewHistoryWidgetViewModel(
    string DescriptionTitle,
    string ReviewerNameTitle,
    string OwnerNameTitle,
    string StateTitle,
    string HasConcreteReviewer,
    string IsOriginalReviewer,
    string BooleanTrueText,
    string BooleanFalseText,
    IReadOnlyDictionary<string, string> StateLookup,
    IReadOnlyCollection<DateSelectorItem> DateItems,
    IReadOnlyCollection<TaskForReviewDto> Tasks)
    : IViewModel<ReviewHistoryWidgetViewModel>
{
    public static ReviewHistoryWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        new Dictionary<string, string>(),
        Array.Empty<DateSelectorItem>(),
        Array.Empty<TaskForReviewDto>());
}