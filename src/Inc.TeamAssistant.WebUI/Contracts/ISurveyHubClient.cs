using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyHubClient
{
    Task FacilitatorChanged(long personId);
    
    Task PersonsChanged(IReadOnlyCollection<Person> persons);
}