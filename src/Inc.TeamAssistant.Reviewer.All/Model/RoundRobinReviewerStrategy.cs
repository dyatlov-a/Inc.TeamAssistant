namespace Inc.TeamAssistant.Reviewer.All.Model;

internal sealed class RoundRobinReviewerStrategy : INextReviewerStrategy
{
    private readonly Team _team;

    public RoundRobinReviewerStrategy(Team team)
    {
        _team = team ?? throw new ArgumentNullException(nameof(team));
    }
    
    public Player Next(Player owner)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));
        
        var lastReviewerId = owner.LastReviewerId ?? long.MinValue;
        var otherPlayers = _team.Players.Where(p => p.Person.Id != owner.Person.Id).ToArray();
        var nextReviewer = otherPlayers.Where(p => p.Person.Id > lastReviewerId).MinBy(p => p.Person.Id);
        return nextReviewer ?? otherPlayers.MinBy(p => p.Person.Id)!;
    }
}