using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Application;

internal static class Messages
{
    public static readonly MessageId Reviewer_NeedReview = new(nameof(Reviewer_NeedReview));
    public static readonly MessageId Reviewer_ReviewDeclined = new(nameof(Reviewer_ReviewDeclined));
    public static readonly MessageId Reviewer_NewTaskForReview = new(nameof(Reviewer_NewTaskForReview));
    public static readonly MessageId Reviewer_Accepted = new(nameof(Reviewer_Accepted));
    public static readonly MessageId Reviewer_MoveToInProgress = new(nameof(Reviewer_MoveToInProgress));
    public static readonly MessageId Reviewer_MoveToAccept = new(nameof(Reviewer_MoveToAccept));
    public static readonly MessageId Reviewer_MoveToDecline = new(nameof(Reviewer_MoveToDecline));
    public static readonly MessageId Reviewer_MoveToNextRound = new(nameof(Reviewer_MoveToNextRound));
    public static readonly MessageId Reviewer_OperationApplied = new(nameof(Reviewer_OperationApplied));
    public static readonly MessageId Reviewer_TaskTitleIsEmpty = new(nameof(Reviewer_TaskTitleIsEmpty));
}