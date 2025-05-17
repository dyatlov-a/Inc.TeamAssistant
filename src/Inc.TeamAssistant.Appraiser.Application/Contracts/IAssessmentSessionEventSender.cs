namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IAssessmentSessionEventSender
{
    Task StoryChanged(Guid teamId);
    
    Task StoryAccepted(Guid teamId, string totalValue);
}