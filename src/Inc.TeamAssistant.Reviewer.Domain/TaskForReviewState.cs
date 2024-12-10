namespace Inc.TeamAssistant.Reviewer.Domain;

public enum TaskForReviewState
{
    New = 1,
    InProgress = 2,
    OnCorrection = 3,
    Accept = 4,
    AcceptWithComments = 5
}