using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public sealed record TaskForReviewHistory(
    Guid Id,
    DateTimeOffset Created,
    TaskForReviewState State,
    Guid BotId,
    string Description,
    IReadOnlyCollection<ReviewInterval> ReviewIntervals,
    long ReviewerId,
    string ReviewerName,
    string? ReviewerUserName,
    long OwnerId,
    string OwnerName,
    string? OwnerUserName,
    bool HasConcreteReviewer,
    bool IsOriginalReviewer)
    : ITaskForReviewStats;