using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyService
{
    Task<GetSurveyTemplatesResult> GetSurveyTemplates(CancellationToken token = default);
}