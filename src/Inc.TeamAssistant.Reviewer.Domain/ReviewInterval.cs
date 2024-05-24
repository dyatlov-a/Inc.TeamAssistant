namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed record ReviewInterval(ReviewIntervalType Type, DateTimeOffset Start, DateTimeOffset End);