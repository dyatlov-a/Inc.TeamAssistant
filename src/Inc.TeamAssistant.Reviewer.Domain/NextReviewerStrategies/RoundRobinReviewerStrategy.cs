namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public sealed class RoundRobinReviewerStrategy : INextReviewerStrategy
{
    private readonly IReadOnlyCollection<long> _teammates;
    private readonly IReadOnlyCollection<long> _excludedPersonIds;
    private readonly long? _lastReviewerId;

    public RoundRobinReviewerStrategy(
        IReadOnlyCollection<long> teammates,
        IReadOnlyCollection<long> excludedPersonIds,
        long? lastReviewerId)
    {
        _teammates = teammates ?? throw new ArgumentNullException(nameof(teammates));
        _excludedPersonIds = excludedPersonIds ?? throw new ArgumentNullException(nameof(excludedPersonIds));
        _lastReviewerId = lastReviewerId;
    }

    public long GetReviewer()
    {
        var otherTeammates = GetOtherTeammates();
        return otherTeammates.Any()
            ? FromOtherTeammates(otherTeammates)
            : _teammates.First();
    }

    private IReadOnlyCollection<long> GetOtherTeammates()
    {
        return _teammates
            .Where(t => !_excludedPersonIds.Contains(t))
            .OrderBy(t => t)
            .ToArray();
    }

    private long FromOtherTeammates(IReadOnlyCollection<long> otherTeammates)
    {
        ArgumentNullException.ThrowIfNull(otherTeammates);
        
        var nextReviewers = otherTeammates
            .Where(ot => _lastReviewerId is null || ot > _lastReviewerId)
            .ToArray();
        var targets = nextReviewers.Any() ? nextReviewers : otherTeammates;

        return targets.MinBy(t => t);
    }
}