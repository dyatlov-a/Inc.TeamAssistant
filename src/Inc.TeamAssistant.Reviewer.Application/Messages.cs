using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Reviewer.Application;

internal static class Messages
{
    public static readonly MessageId Reviewer_NeedReview = new(nameof(Reviewer_NeedReview));
    public static readonly MessageId Reviewer_ReviewDeclined = new(nameof(Reviewer_ReviewDeclined));
    public static readonly MessageId Reviewer_NewTaskForReview = new(nameof(Reviewer_NewTaskForReview));
    public static readonly MessageId Reviewer_Owner = new(nameof(Reviewer_Owner));
    public static readonly MessageId Reviewer_TargetAutomatically = new(nameof(Reviewer_TargetAutomatically));
    public static readonly MessageId Reviewer_TargetManually = new(nameof(Reviewer_TargetManually));
    public static readonly MessageId Reviewer_Accepted = new(nameof(Reviewer_Accepted));
    public static readonly MessageId Reviewer_TotalTime = new(nameof(Reviewer_TotalTime));
    public static readonly MessageId Reviewer_MoveToInProgress = new(nameof(Reviewer_MoveToInProgress));
    public static readonly MessageId Reviewer_MoveToAccept = new(nameof(Reviewer_MoveToAccept));
    public static readonly MessageId Reviewer_MoveToDecline = new(nameof(Reviewer_MoveToDecline));
    public static readonly MessageId Reviewer_MoveToNextRound = new(nameof(Reviewer_MoveToNextRound));
    public static readonly MessageId Connector_TeamNotFound = new(nameof(Connector_TeamNotFound));
    public static readonly MessageId Connector_PersonNotFound = new(nameof(Connector_PersonNotFound));
    public static readonly MessageId Reviewer_TeamWithoutUsers = new(nameof(Reviewer_TeamWithoutUsers));
    public static readonly MessageId Reviewer_Reassign = new(nameof(Reviewer_Reassign));
    public static readonly MessageId Reviewer_NeedEndReview = new(nameof(Reviewer_NeedEndReview));
    public static readonly MessageId Reviewer_NeedCorrection = new(nameof(Reviewer_NeedCorrection));
    public static readonly MessageId Reviewer_StatsAttempts = new(nameof(Reviewer_StatsAttempts));
    public static readonly MessageId Reviewer_StatsFirstTouch = new(nameof(Reviewer_StatsFirstTouch));
    public static readonly MessageId Reviewer_StatsFirstTouchAverage = new(nameof(Reviewer_StatsFirstTouchAverage));
    public static readonly MessageId Reviewer_StatsReview = new(nameof(Reviewer_StatsReview));
    public static readonly MessageId Reviewer_StatsReviewAverage = new(nameof(Reviewer_StatsReviewAverage));
    public static readonly MessageId Reviewer_StatsCorrection = new(nameof(Reviewer_StatsCorrection));
    public static readonly MessageId Reviewer_StatsCorrectionAverage = new(nameof(Reviewer_StatsCorrectionAverage));
    public static readonly MessageId Reviewer_PreviewTitle = new(nameof(Reviewer_PreviewTitle));
    public static readonly MessageId Reviewer_PreviewReviewerTemplate = new(nameof(Reviewer_PreviewReviewerTemplate));
    public static readonly MessageId Reviewer_PreviewCheckDescription = new(nameof(Reviewer_PreviewCheckDescription));
    public static readonly MessageId Reviewer_PreviewCheckTeammate = new(nameof(Reviewer_PreviewCheckTeammate));
    public static readonly MessageId Reviewer_PreviewEditHelp = new(nameof(Reviewer_PreviewEditHelp));
    public static readonly MessageId Reviewer_PreviewMoveToReview = new(nameof(Reviewer_PreviewMoveToReview));
    public static readonly MessageId Reviewer_PreviewRemoveDraft = new(nameof(Reviewer_PreviewRemoveDraft));
}