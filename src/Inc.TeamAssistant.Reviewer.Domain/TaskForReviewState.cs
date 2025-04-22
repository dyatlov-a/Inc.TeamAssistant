namespace Inc.TeamAssistant.Reviewer.Domain;

public enum TaskForReviewState
{
    New = 1,
    InProgress = 2,
    OnCorrection = 3,
    FirstAccept = 4,
    Accept = 5,
    AcceptWithComments = 6
}