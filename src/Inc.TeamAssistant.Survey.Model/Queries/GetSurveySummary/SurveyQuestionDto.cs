namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record SurveyQuestionDto(
    string Title,
    string Text,
    int Mean,
    IReadOnlyCollection<PersonAnswerDto> Answers,
    IReadOnlyCollection<int> MeanHistory);