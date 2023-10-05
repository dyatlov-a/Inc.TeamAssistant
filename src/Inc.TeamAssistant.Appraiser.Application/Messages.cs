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
    public static readonly MessageId AllowUseNameHelp = new(nameof(AllowUseNameHelp));
    public static readonly MessageId ShowParticipantsHelp = new(nameof(ShowParticipantsHelp));
    public static readonly MessageId AddStoryToAssessmentSessionHelp = new(nameof(AddStoryToAssessmentSessionHelp));
    public static readonly MessageId ChangeLanguageHelp = new(nameof(ChangeLanguageHelp));
    public static readonly MessageId AcceptEstimateHelp = new(nameof(AcceptEstimateHelp));
    public static readonly MessageId ReVoteEstimateHelp = new(nameof(ReVoteEstimateHelp));
    public static readonly MessageId FinishAssessmentSessionHelp = new(nameof(FinishAssessmentSessionHelp));
    public static readonly MessageId JoinToAssessmentSessionHelp = new(nameof(JoinToAssessmentSessionHelp));
}