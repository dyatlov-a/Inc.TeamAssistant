namespace Inc.TeamAssistant.Survey.Domain;

public sealed record Answer(
    Guid QuestionId,
    int? Value,
    string? Comment);