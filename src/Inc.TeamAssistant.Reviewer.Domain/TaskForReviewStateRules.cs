namespace Inc.TeamAssistant.Reviewer.Domain;

public static class TaskForReviewStateRules
{
    public static readonly IReadOnlyCollection<TaskForReviewState> ActiveStates =
    [
        TaskForReviewState.New,
        TaskForReviewState.InProgress,
        TaskForReviewState.OnCorrection
    ];
}