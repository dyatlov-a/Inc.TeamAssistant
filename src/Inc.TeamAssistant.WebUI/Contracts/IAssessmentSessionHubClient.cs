namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IAssessmentSessionHubClient
{
    Task StoryChanged();

    Task StoryAccepted(string totalValue);
}