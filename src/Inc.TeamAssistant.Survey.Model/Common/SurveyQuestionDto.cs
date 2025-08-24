namespace Inc.TeamAssistant.Survey.Model.Common;

public sealed record SurveyQuestionDto(
    Guid QuestionId,
    string QuestionTitle,
    string QuestionText,
    DateTimeOffset SurveyDate,
    long ResponderId,
    int Value,
    string? Comment);