using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed record LastTasksWidgetViewModel(
    string CreatedTitle,
    string DescriptionTitle,
    string ReviewerNameTitle,
    string OwnerNameTitle,
    string StateTitle,
    IReadOnlyCollection<TaskForReviewDto> Tasks)
    : IViewModel<LastTasksWidgetViewModel>
{
    public static LastTasksWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<TaskForReviewDto>());
}