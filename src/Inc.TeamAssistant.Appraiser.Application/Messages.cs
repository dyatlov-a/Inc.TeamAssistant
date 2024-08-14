using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Application;

internal static class Messages
{
    public static readonly MessageId Appraiser_EndEstimate = new(nameof(Appraiser_EndEstimate));
    public static readonly MessageId Appraiser_NeedEstimate = new(nameof(Appraiser_NeedEstimate));
    public static readonly MessageId Appraiser_AverageEstimate = new(nameof(Appraiser_AverageEstimate));
    public static readonly MessageId Appraiser_AcceptedEstimate = new(nameof(Appraiser_AcceptedEstimate));
    public static readonly MessageId Appraiser_StoryNotFound = new(nameof(Appraiser_StoryNotFound));
    public static readonly MessageId Appraiser_Accept = new(nameof(Appraiser_Accept));
    public static readonly MessageId Appraiser_Finish = new(nameof(Appraiser_Finish));
    public static readonly MessageId Appraiser_Revote = new(nameof(Appraiser_Revote));
    
    public static readonly MessageId Connector_TeamNotFound = new(nameof(Connector_TeamNotFound));
}