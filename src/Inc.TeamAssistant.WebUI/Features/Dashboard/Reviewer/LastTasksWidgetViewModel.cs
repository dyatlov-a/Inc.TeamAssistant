using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed record LastTasksWidgetViewModel(
    string DescriptionTitle,
    string ReviewerNameTitle,
    string OwnerNameTitle,
    string StateTitle,
    IReadOnlyDictionary<string, string> StateLookup,
    IReadOnlyCollection<DateSelectorItem> DateItems,
    IReadOnlyCollection<TaskForReviewDto> Tasks)
    : IViewModel<LastTasksWidgetViewModel>
{
    public static LastTasksWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        new Dictionary<string, string>(),
        Array.Empty<DateSelectorItem>(),
        Array.Empty<TaskForReviewDto>());
}