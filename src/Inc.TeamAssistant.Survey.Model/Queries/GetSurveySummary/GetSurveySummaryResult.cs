namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record GetSurveySummaryResult(IReadOnlyCollection<SurveyQuestionDto> Items);