namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

internal sealed class RoundRobinReviewerStrategy : INextReviewerStrategy
{
    private readonly IReadOnlyCollection<long> _teammates;

    public RoundRobinReviewerStrategy(IReadOnlyCollection<long> teammates)
    {
        _teammates = teammates ?? throw new ArgumentNullException(nameof(teammates));
    }
    
    public long Next(long ownerId, long? lastReviewerId)
    {
        var otherTeammates = _teammates.Where(t => t != ownerId).OrderBy(t => t).ToArray();

        if (!otherTeammates.Any())
            return _teammates.First();

        var nextReviewers = otherTeammates.Where(ot => lastReviewerId is null || ot > lastReviewerId).ToArray();
        var targets = nextReviewers.Any() ? nextReviewers : otherTeammates;
        return targets.MinBy(t => t);
    }
}