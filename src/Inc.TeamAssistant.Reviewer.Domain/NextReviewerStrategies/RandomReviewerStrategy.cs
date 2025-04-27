namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public sealed class RandomReviewerStrategy : INextReviewerStrategy
{
    private const int ReviewerWeight = 100;

    private readonly TeammatesPool _teammatesPool;
    private readonly IReadOnlyDictionary<long, int> _history;

    public RandomReviewerStrategy(TeammatesPool teammatesPool, IReadOnlyDictionary<long, int> history)
    {
        _teammatesPool = teammatesPool ?? throw new ArgumentNullException(nameof(teammatesPool));
        _history = history ?? throw new ArgumentNullException(nameof(history));
    }

    public long GetReviewer()
    {
        var targetPlayers = GetTargetPlayers();
        return targetPlayers.Any()
            ? FromTargetPlayers(targetPlayers)
            : _teammatesPool.Teammates.First();
    }

    private IReadOnlyCollection<long> GetTargetPlayers()
    {
        var teammatesWithoutParticipates = _teammatesPool.WithoutParticipates();
        var results = teammatesWithoutParticipates.Any()
            ? teammatesWithoutParticipates
            : _teammatesPool.OnlyOtherTeammates();
        
        return results;
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