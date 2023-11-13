using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application;

internal static class Messages
{
    public static readonly MessageId SessionNotFoundForModerator = new(nameof(SessionNotFoundForModerator));
    public static readonly MessageId SessionNotFoundForAppraiser = new(nameof(SessionNotFoundForAppraiser));
    public static readonly MessageId AppraiserConnectWithError = new(nameof(AppraiserConnectWithError));
    public static readonly MessageId AppraiserConnectedToOtherSession = new(nameof(AppraiserConnectedToOtherSession));
    public static readonly MessageId EndAssessmentWithError = new(nameof(EndAssessmentWithError));

    public static readonly MessageId Error_StoryTitleIsEmpty = new(nameof(Error_StoryTitleIsEmpty));
    public static readonly MessageId Error_StoryLinkFormat = new(nameof(Error_StoryLinkFormat));
    public static readonly MessageId Error_StarterMessage = new(nameof(Error_StarterMessage));

    public static readonly MessageId ExitFromAssessmentSessionHelp = new(nameof(ExitFromAssessmentSessionHelp));
    public static readonly MessageId CreateAssessmentSessionHelp = new(nameof(CreateAssessmentSessionHelp));
    public static readonly MessageId AddStoryToAssessmentSessionHelp = new(nameof(AddStoryToAssessmentSessionHelp));
    public static readonly MessageId ChangeLanguageHelp = new(nameof(ChangeLanguageHelp));
    public static readonly MessageId AcceptEstimateHelp = new(nameof(AcceptEstimateHelp));
    public static readonly MessageId ReVoteEstimateHelp = new(nameof(ReVoteEstimateHelp));
    public static readonly MessageId FinishAssessmentSessionHelp = new(nameof(FinishAssessmentSessionHelp));
    
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
    public static readonly MessageId EndEstimate = new(nameof(EndEstimate));
    public static readonly MessageId NeedEstimate = new(nameof(NeedEstimate));
    public static readonly MessageId TotalEstimate = new(nameof(TotalEstimate));
}