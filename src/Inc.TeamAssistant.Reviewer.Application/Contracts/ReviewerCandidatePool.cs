namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public sealed record ReviewerCandidatePool(
    IReadOnlyDictionary<long, int> FirstRoundStats,
    IReadOnlyCollection<ReviewerCandidatePool.FirstRoundHistoryItem> FirstRoundHistory,
    long? SecondRoundHistory)
{
    public sealed record FirstRoundHistoryItem(
        long ReviewerId,
        long OwnerId,
        DateTimeOffset Created);
}