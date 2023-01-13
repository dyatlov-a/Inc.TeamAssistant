namespace Inc.TeamAssistant.Reviewer.All.Model;

public static class TaskForReviewStateRules
{
    public static readonly IReadOnlyCollection<TaskForReviewState> ActiveStates = new[]
    {
        TaskForReviewState.New,
        TaskForReviewState.InProgress,
        TaskForReviewState.OnCorrection
    };
}