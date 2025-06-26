using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyReader
{
    Task<IReadOnlyCollection<SurveyTemplate>> GetTemplates(CancellationToken token);
}