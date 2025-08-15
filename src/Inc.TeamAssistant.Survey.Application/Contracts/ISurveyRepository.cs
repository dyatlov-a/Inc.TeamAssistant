using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyRepository
{
    Task<SurveyEntry?> Find(Guid surveyId, CancellationToken token);
    
    Task<SurveyAnswer?> Find(Guid surveyId, long ownerId, CancellationToken token);
    
    Task Upsert(SurveyEntry survey, CancellationToken token);
    
    Task Upsert(SurveyAnswer surveyAnswer, CancellationToken token);
}