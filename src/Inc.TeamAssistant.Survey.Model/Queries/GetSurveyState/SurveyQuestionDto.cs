namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

public sealed record SurveyQuestionDto(
    Guid Id,
    string Title,
    string Text,
    int? Value,
    string? Comment);