namespace Inc.TeamAssistant.Appraiser.Backend.Hubs;

public interface IMessagesHubClient
{
    Task StoryChanged();
}