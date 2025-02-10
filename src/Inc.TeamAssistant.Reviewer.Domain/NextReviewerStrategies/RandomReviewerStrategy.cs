namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public sealed class RandomReviewerStrategy : INextReviewerStrategy
{
    private const int ReviewerWeight = 100;

    private readonly IReadOnlyCollection<long> _teammates;
    private readonly IReadOnlyDictionary<long, int> _history;
    private readonly IReadOnlyCollection<long> _excludedPersonIds;
    private readonly long? _lastReviewerId;

    public RandomReviewerStrategy(
        IReadOnlyCollection<long> teammates,
        IReadOnlyDictionary<long, int> history,
        IReadOnlyCollection<long> excludedPersonIds,
        long? lastReviewerId)
    {
        _teammates = teammates ?? throw new ArgumentNullException(nameof(teammates));
        _history = history ?? throw new ArgumentNullException(nameof(history));
        _excludedPersonIds = excludedPersonIds ?? throw new ArgumentNullException(nameof(excludedPersonIds));
        _lastReviewerId = lastReviewerId;
    }

    public long GetReviewer()
    {
        var targetPlayers = GetTargetPlayers();
        return targetPlayers.Any()
            ? FromTargetPlayers(targetPlayers)
            : _teammates.First();
    }

    private IReadOnlyCollection<long> GetTargetPlayers()
    {
        var excludedTeammates = _lastReviewerId.HasValue
            ? _excludedPersonIds.Append(_lastReviewerId.Value)
            : _excludedPersonIds;
        
        return _teammates
            .Where(t => !excludedTeammates.Contains(t))
            .OrderBy(t => t)
            .ToArray();
    }

    private long FromTargetPlayers(IReadOnlyCollection<long> targetPlayers)
    {
        ArgumentNullException.ThrowIfNull(targetPlayers);
        
        var seeding = targetPlayers
            .Select(t => (PersonId: t, Count: 1d / _history.GetValueOrDefault(t, 1) * ReviewerWeight))
            .SelectMany(t => Enumerable.Repeat(t.PersonId, (int)t.Count))
            .ToArray();
        
        var nextReviewerIndex = Random.Shared.Next(0, seeding.Length);
        return seeding[nextReviewerIndex];
    }
}