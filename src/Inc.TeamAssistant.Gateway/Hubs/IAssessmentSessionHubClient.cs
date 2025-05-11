namespace Inc.TeamAssistant.Gateway.Hubs;

public interface IAssessmentSessionHubClient
{
    Task StoryChanged();

    Task StoryAccepted(string totalValue);
}