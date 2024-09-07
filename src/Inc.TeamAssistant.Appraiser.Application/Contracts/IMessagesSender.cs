namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IMessagesSender
{
    Task StoryChanged(Guid teamId);
    
    Task StoryAccepted(Guid teamId, string totalValue);
}