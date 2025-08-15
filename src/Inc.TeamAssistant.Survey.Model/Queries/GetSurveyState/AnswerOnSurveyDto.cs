namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

public sealed record AnswerOnSurveyDto(
    Guid Id,
    string Title,
    string Text,
    int? Value,
    string? Comment);