namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

public sealed record TaskForReviewDto(
    Guid Id,
    DateTimeOffset Created,
    string State,
    string Description,
    TimeSpan FirstTouch,
    TimeSpan Correction,
    TimeSpan Review,
    int Iterations,
    long ReviewerId,
    string ReviewerName,
    string? ReviewerUserName,
    long OwnerId,
    string OwnerName,
    string? OwnerUserName,
    bool HasConcreteReviewer,
    bool IsOriginalReviewer)
{
    public TimeSpan TotalTime => FirstTouch + Correction + Review;
}