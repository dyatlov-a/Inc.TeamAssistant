namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

internal sealed class RoundRobinReviewerStrategy : INextReviewerStrategy
{
    private readonly Team _team;

    public RoundRobinReviewerStrategy(Team team)
    {
        _team = team ?? throw new ArgumentNullException(nameof(team));
    }
    
    public Person Next(Person owner, Person? lastReviewer)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));
        
        var otherPlayers = _team.Players.Where(p => p.Id != owner.Id).ToArray();

        if (!otherPlayers.Any())
            return _team.Players.First();

        var nextReviewers = otherPlayers.Where(p => lastReviewer is null || p.Id > lastReviewer.Id).ToArray();
        var targets = nextReviewers.Any() ? nextReviewers : otherPlayers;
        return targets.MinBy(p => p.Id)!;
    }
}