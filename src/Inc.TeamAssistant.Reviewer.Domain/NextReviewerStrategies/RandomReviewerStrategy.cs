namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

internal sealed class RandomReviewerStrategy : INextReviewerStrategy
{
    private static readonly Random RandomSelector = new();

    private readonly IReadOnlyCollection<long> _teammates;

    public RandomReviewerStrategy(IReadOnlyCollection<long> teammates)
    {
        _teammates = teammates ?? throw new ArgumentNullException(nameof(teammates));
    }
    
    public long Next(long ownerId, long? lastReviewerId)
    {
        var excludedTeammates = lastReviewerId.HasValue ? new[] { ownerId, lastReviewerId.Value } : new[] { ownerId };
        var targetPlayers = _teammates
            .Where(t => !excludedTeammates.Contains(t))
            .OrderBy(t => t)
            .ToArray();

        if (!targetPlayers.Any())
            return _teammates.First();
        
        var nextReviewerIndex = RandomSelector.Next(0, targetPlayers.Length);
        return targetPlayers[nextReviewerIndex];
    }
}