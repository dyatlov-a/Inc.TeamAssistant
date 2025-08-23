using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyReader
{
    Task<IReadOnlyCollection<Question>> ReadQuestions(Guid templateId, CancellationToken token);
    
    Task<SurveyEntry?> ReadLastSurvey(Guid roomId, IReadOnlyCollection<SurveyState> states, CancellationToken token);
    
    Task<IReadOnlyCollection<SurveyEntry>> ReadSurveys(
        DateTimeOffset from,
        Guid templateId,
        SurveyState state,
        int offset,
        int limit,
        CancellationToken token);
    
    Task<IReadOnlyCollection<SurveyAnswer>> ReadAnswers(IReadOnlyCollection<Guid> surveyIds, CancellationToken token);
}