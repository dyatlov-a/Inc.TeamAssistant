namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed record ReviewComment(
    DateTimeOffset Created,
    string Comment,
    long AuthorId);