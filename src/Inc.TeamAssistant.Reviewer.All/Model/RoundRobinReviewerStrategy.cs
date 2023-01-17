namespace Inc.TeamAssistant.Reviewer.All.Model;

internal sealed class RoundRobinReviewerStrategy : INextReviewerStrategy
{
    private readonly Team _team;

    public RoundRobinReviewerStrategy(Team team)
    {
        _team = team ?? throw new ArgumentNullException(nameof(team));
    }
    
    public Player Next(Person owner, Person? lastReviewer)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));
        
        var otherPlayers = _team.Players.Where(p => p.Person.Id != owner.Id).ToArray();

        if (!otherPlayers.Any())
            return _team.Players.First();
        
        var nextReviewer = otherPlayers
            .Where(p => lastReviewer is null || p.Person.Id > lastReviewer.Id)
            .MinBy(p => p.Person.Id);
        return nextReviewer ?? otherPlayers.MinBy(p => p.Person.Id)!;
    }
}