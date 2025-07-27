using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyReader
{
    Task<IReadOnlyCollection<Question>> ReadQuestions(IReadOnlyCollection<Guid> questionIds, CancellationToken token);
    
    Task<SurveyTemplate?> FindTemplate(Guid id, CancellationToken token);
    
    Task<SurveyEntry?> Find(Guid roomId, IReadOnlyCollection<SurveyState> states, CancellationToken token);
}