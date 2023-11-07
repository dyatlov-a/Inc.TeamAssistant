using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IMessagesSender
{
    Task StoryChanged(AssessmentSessionId assessmentSessionId);
}