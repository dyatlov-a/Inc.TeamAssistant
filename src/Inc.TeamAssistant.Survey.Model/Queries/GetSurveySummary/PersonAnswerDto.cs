namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record PersonAnswerDto(
    Guid QuestionId,
    string QuestionKey,
    long PersonId,
    int Value,
    string? Comment);