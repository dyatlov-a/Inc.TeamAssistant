namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record SurveyQuestionDto(
    Guid QuestionId,
    string QuestionTitle,
    string QuestionText,
    DateTimeOffset SurveyDate,
    long ResponderId,
    int Value,
    string? Comment);