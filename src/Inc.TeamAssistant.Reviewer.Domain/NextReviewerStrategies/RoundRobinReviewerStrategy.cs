namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public sealed class RoundRobinReviewerStrategy : INextReviewerStrategy
{
    private readonly TeammatesPool _teammatesPool;

    public RoundRobinReviewerStrategy(TeammatesPool teammatesPool)
    {
        _teammatesPool = teammatesPool ?? throw new ArgumentNullException(nameof(teammatesPool));
    }

    public long GetReviewer()
    {
        var otherTeammates = _teammatesPool.OnlyOtherTeammates();
        var reviewer = otherTeammates.Any()
            ? FromOtherTeammates(otherTeammates)
            : _teammatesPool.Teammates.First();

        return reviewer;
    }

    private long FromOtherTeammates(IReadOnlyCollection<long> otherTeammates)
    {
        ArgumentNullException.ThrowIfNull(otherTeammates);
        
        var nextReviewers = otherTeammates
            .Where(ot => !_teammatesPool.LastReviewerId.HasValue || ot > _teammatesPool.LastReviewerId.Value)
            .ToArray();
        var targets = nextReviewers.Any() ? nextReviewers : otherTeammates;
        var reviewer = targets.MinBy(t => t);

        return reviewer;
    }
}