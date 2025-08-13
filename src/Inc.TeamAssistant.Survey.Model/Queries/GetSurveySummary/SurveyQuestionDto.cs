namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record SurveyQuestionDto(
    Guid Id,
    string Title,
    string Text,
    IReadOnlyCollection<QuestionAnswerDto> Answers);