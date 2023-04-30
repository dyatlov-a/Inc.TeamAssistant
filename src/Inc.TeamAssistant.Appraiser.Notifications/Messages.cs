using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Notifications;

internal static class Messages
{
    public static readonly MessageId EnterSessionName = new(nameof(EnterSessionName));
    public static readonly MessageId CreateAssessmentSessionFailed = new(nameof(CreateAssessmentSessionFailed));
    public static readonly MessageId EnterStoryName = new(nameof(EnterStoryName));
    public static readonly MessageId LanguageChanged = new(nameof(LanguageChanged));
    public static readonly MessageId ConnectToSession = new(nameof(ConnectToSession));
    public static readonly MessageId ConnectedSuccess = new(nameof(ConnectedSuccess));
    public static readonly MessageId ConnectToDashboard = new(nameof(ConnectToDashboard));
    public static readonly MessageId AppraiserAdded = new(nameof(AppraiserAdded));
    public static readonly MessageId DisconnectedFromSession = new(nameof(DisconnectedFromSession));
    public static readonly MessageId AppraiserDisconnectedFromSession = new(nameof(AppraiserDisconnectedFromSession));
    public static readonly MessageId SessionEnded = new(nameof(SessionEnded));
    public static readonly MessageId EstimateRepeated = new(nameof(EstimateRepeated));
    public static readonly MessageId EndEstimate = new(nameof(EndEstimate));
    public static readonly MessageId NeedEstimate = new(nameof(NeedEstimate));
    public static readonly MessageId TotalEstimate = new(nameof(TotalEstimate));
    public static readonly MessageId AppraiserList = new(nameof(AppraiserList));
    public static readonly MessageId Loading = new(nameof(Loading));
    public static readonly MessageId EnterSessionId = new(nameof(EnterSessionId));
}