namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

public sealed record GetLastTasksResult(IReadOnlyCollection<TaskForReviewDto> Items);