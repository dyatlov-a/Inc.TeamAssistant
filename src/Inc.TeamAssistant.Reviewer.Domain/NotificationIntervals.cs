namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed record NotificationIntervals(TimeSpan Waiting, TimeSpan InProgress)
{
    public TimeSpan GetNotificationInterval(TaskForReviewState state)
    {
        return state == TaskForReviewState.InProgress
            ? InProgress
            : Waiting;
    }
}