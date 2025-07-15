namespace Inc.TeamAssistant.Survey.Model.Queries.GetPersonSurvey;

public sealed record SurveyQuestionDto(
    Guid Id,
    string Title,
    string Text,
    int? Value,
    string? Comment);