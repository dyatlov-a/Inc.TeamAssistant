namespace Inc.TeamAssistant.Survey.Model.Queries.GetPersonSurvey;

public sealed record GetPersonSurveyResult(IReadOnlyCollection<SurveyQuestionDto> Questions);