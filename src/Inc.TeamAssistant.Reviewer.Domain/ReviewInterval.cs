namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed record ReviewInterval(
    TaskForReviewState State,
    DateTimeOffset Begin,
    DateTimeOffset End,
    long UserId);