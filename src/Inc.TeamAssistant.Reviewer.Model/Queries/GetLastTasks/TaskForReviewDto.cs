namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

public sealed record TaskForReviewDto(
    string State,
    Guid Id,
    DateTimeOffset Created,
    string Description,
    long ReviewerId,
    string ReviewerName,
    string? ReviewerUserName,
    long OwnerId,
    string OwnerName,
    string? OwnerUserName,
    bool HasConcreteReviewer,
    bool IsOriginalReviewer);