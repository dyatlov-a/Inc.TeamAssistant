namespace Inc.TeamAssistant.Reviewer.Domain;

public interface ITaskForReviewStats
{
    DateTimeOffset Created { get; }
    IReadOnlyCollection<ReviewInterval> ReviewIntervals { get; }
    Guid BotId { get; }
    long ReviewerId { get; }
    long OwnerId { get; }
}