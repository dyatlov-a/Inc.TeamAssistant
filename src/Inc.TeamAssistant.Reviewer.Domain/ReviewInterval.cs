namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed record ReviewInterval(TaskForReviewState State, DateTimeOffset End, long UserId);