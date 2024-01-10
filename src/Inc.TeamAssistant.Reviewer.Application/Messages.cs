using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application;

internal static class Messages
{
    public static readonly MessageId Reviewer_CreateTeamHelp = new(nameof(Reviewer_CreateTeamHelp));
    public static readonly MessageId Reviewer_MoveToReviewHelp = new(nameof(Reviewer_MoveToReviewHelp));
    public static readonly MessageId Reviewer_EnterNextReviewerType = new(nameof(Reviewer_EnterNextReviewerType));
    public static MessageId Reviewer_NextReviewerType(NextReviewerType value) => new($"{nameof(Reviewer_NextReviewerType)}_{value}");
    public static readonly MessageId Reviewer_EnterTeamName = new(nameof(Reviewer_EnterTeamName));
    public static readonly MessageId Reviewer_ConnectToTeam = new(nameof(Reviewer_ConnectToTeam));
    public static readonly MessageId Reviewer_SelectTeam = new(nameof(Reviewer_SelectTeam));
    public static readonly MessageId Reviewer_EnterRequestForReview = new(nameof(Reviewer_EnterRequestForReview));
    public static readonly MessageId Reviewer_TeamMinError = new(nameof(Reviewer_TeamMinError));
    public static readonly MessageId Reviewer_JoinToTeamSuccess = new(nameof(Reviewer_JoinToTeamSuccess));
    public static readonly MessageId Reviewer_LeaveTeamSuccess = new(nameof(Reviewer_LeaveTeamSuccess));
    public static readonly MessageId Reviewer_JoinSuccess = new(nameof(Reviewer_JoinSuccess));
    public static readonly MessageId Reviewer_TeamNotFoundError = new(nameof(Reviewer_TeamNotFoundError));
    public static readonly MessageId Reviewer_NeedReview = new(nameof(Reviewer_NeedReview));
    public static readonly MessageId Reviewer_ReviewDeclined = new(nameof(Reviewer_ReviewDeclined));
    public static readonly MessageId Reviewer_NewTaskForReview = new(nameof(Reviewer_NewTaskForReview));
    public static readonly MessageId Reviewer_LeaveHelp = new(nameof(Reviewer_LeaveHelp));
    public static readonly MessageId Reviewer_CancelHelp = new(nameof(Reviewer_CancelHelp));
    public static readonly MessageId Reviewer_CancelDialogFail = new(nameof(Reviewer_CancelDialogFail));
    public static readonly MessageId Reviewer_BeginDialogFail = new(nameof(Reviewer_BeginDialogFail));
    public static readonly MessageId Reviewer_Accepted = new(nameof(Reviewer_Accepted));
    public static readonly MessageId Reviewer_MoveToInProgress = new(nameof(Reviewer_MoveToInProgress));
    public static readonly MessageId Reviewer_MoveToAccept = new(nameof(Reviewer_MoveToAccept));
    public static readonly MessageId Reviewer_MoveToDecline = new(nameof(Reviewer_MoveToDecline));
    public static readonly MessageId Reviewer_MoveToNextRound = new(nameof(Reviewer_MoveToNextRound));
    public static readonly MessageId Reviewer_OperationApplied = new(nameof(Reviewer_OperationApplied));
    public static readonly MessageId Reviewer_HasNotTeamsForPlayer = new(nameof(Reviewer_HasNotTeamsForPlayer));
}