using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyRepository
{
    Task Upsert(SurveyEntry survey, CancellationToken token);
}