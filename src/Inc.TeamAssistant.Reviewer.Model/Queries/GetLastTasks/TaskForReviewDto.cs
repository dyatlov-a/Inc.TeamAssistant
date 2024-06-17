namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

public sealed record TaskForReviewDto(
    Guid Id,
    DateTimeOffset Created,
    string Description,
    string ReviewerName,
    string OwnerName,
    string State);