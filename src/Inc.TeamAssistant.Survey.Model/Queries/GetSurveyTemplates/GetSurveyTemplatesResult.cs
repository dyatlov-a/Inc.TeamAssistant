namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;

public sealed record GetSurveyTemplatesResult(IReadOnlyCollection<SurveyTemplateDto> Templates);