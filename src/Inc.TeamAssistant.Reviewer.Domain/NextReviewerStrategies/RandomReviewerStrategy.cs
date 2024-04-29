namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

internal sealed class RandomReviewerStrategy : INextReviewerStrategy
{
    private const int ReviewerWeight = 100;

    private readonly IReadOnlyCollection<long> _teammates;
    private readonly IReadOnlyDictionary<long, int> _history;

    public RandomReviewerStrategy(IReadOnlyCollection<long> teammates, IReadOnlyDictionary<long, int> history)
    {
        _teammates = teammates ?? throw new ArgumentNullException(nameof(teammates));
        _history = history ?? throw new ArgumentNullException(nameof(history));
    }
    
    public long Next(IReadOnlyCollection<long> excludedPersonIds, long? lastReviewerId)
    {
        ArgumentNullException.ThrowIfNull(excludedPersonIds);
        
        var excludedTeammates = lastReviewerId.HasValue
            ? excludedPersonIds.Append(lastReviewerId.Value)
            : excludedPersonIds;
        var targetPlayers = _teammates
            .Where(t => !excludedTeammates.Contains(t))
            .OrderBy(t => t)
            .ToArray();

        if (!targetPlayers.Any())
            return _teammates.First();
        
        var seeding = targetPlayers
            .Select(t => (PersonId: t, Count: 1d / _history.GetValueOrDefault(t, 1) * ReviewerWeight))
            .SelectMany(t => Enumerable.Repeat(t.PersonId, (int)t.Count))
            .ToArray();
        
        var nextReviewerIndex = Random.Shared.Next(0, seeding.Length);
        return seeding[nextReviewerIndex];
    }
}