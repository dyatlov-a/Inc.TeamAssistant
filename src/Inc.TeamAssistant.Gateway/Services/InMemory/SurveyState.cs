using System.Collections.Concurrent;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class SurveyState : ISurveyState
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<long, SurveyAnswer>> _state = new();
    
    public SurveyAnswer? Get(Guid surveyId, long ownerId)
    {
        return _state.TryGetValue(surveyId, out var answers) && answers.TryGetValue(ownerId, out var answer)
            ? answer
            : null;
    }

    public void Set(SurveyAnswer surveyAnswer)
    {
        ArgumentNullException.ThrowIfNull(surveyAnswer);
        
        var answers = _state.GetOrAdd(
            surveyAnswer.SurveyId,
            _ => new ConcurrentDictionary<long, SurveyAnswer>());
        
        answers.AddOrUpdate(surveyAnswer.OwnerId, k => surveyAnswer, (k, v) => surveyAnswer);
    }
    
    public void Clear(Guid surveyId) => _state.TryRemove(surveyId, out _);
}