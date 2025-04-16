using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

public sealed record GetLastTasksResult(IReadOnlyCollection<TaskForReviewDto> Items)
    : IWithEmpty<GetLastTasksResult>
{
    public static GetLastTasksResult Empty { get; } = new(Array.Empty<TaskForReviewDto>());
}