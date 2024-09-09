namespace Inc.TeamAssistant.Gateway.Hubs;

public interface IMessagesHubClient
{
    Task StoryChanged();

    Task StoryAccepted(string totalValue);
}