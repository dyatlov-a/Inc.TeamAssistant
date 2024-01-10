using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application;

internal static class Messages
{
    public static readonly MessageId Appraiser_StoryTitleIsEmpty = new(nameof(Appraiser_StoryTitleIsEmpty));
    public static readonly MessageId Appraiser_EnterStoryName = new(nameof(Appraiser_EnterStoryName));
    public static readonly MessageId Appraiser_EndEstimate = new(nameof(Appraiser_EndEstimate));
    public static readonly MessageId Appraiser_NeedEstimate = new(nameof(Appraiser_NeedEstimate));
    public static readonly MessageId Appraiser_TotalEstimate = new(nameof(Appraiser_TotalEstimate));
    
    public static readonly MessageId Connector_TeamNotFound = new(nameof(Connector_TeamNotFound));
    public static readonly MessageId Connector_SelectTeam = new(nameof(Connector_SelectTeam));
}