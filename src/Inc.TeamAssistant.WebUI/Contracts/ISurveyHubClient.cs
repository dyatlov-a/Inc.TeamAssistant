namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyHubClient
{
    Task FacilitatorChanged(long personId);
}