namespace Inc.TeamAssistant.Reviewer.All.Model;

public static class TaskForReviewStateRules
{
    public static IReadOnlyCollection<TaskForReviewState> ActiveStates = new[]
    {
        TaskForReviewState.InProgress,
        TaskForReviewState.OnCorrection
    };
}