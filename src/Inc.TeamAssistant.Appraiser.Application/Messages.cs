using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Application;

internal static class Messages
{
    public static readonly MessageId Appraiser_EndEstimate = new(nameof(Appraiser_EndEstimate));
    public static readonly MessageId Appraiser_NeedEstimate = new(nameof(Appraiser_NeedEstimate));
    public static readonly MessageId Appraiser_TotalEstimate = new(nameof(Appraiser_TotalEstimate));
    public static readonly MessageId Appraiser_StoryNotFound = new(nameof(Appraiser_StoryNotFound));
    public static readonly MessageId Connector_TeamNotFound = new(nameof(Connector_TeamNotFound));
}