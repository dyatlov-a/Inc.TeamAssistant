using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyHubClient
{
    Task SurveyStarted();
    
    Task FacilitatorChanged(long personId);
    
    Task PersonsChanged(IReadOnlyCollection<PersonStateTicket> tickets);
    
    Task SurveyStateChanged(long personId, bool finished);

    Task SurveyFinished();
}