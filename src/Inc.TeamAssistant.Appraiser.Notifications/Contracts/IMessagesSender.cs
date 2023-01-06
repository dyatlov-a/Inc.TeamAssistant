using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Notifications.Contracts;

public interface IMessagesSender
{
    Task StoryChanged(AssessmentSessionId assessmentSessionId);
}