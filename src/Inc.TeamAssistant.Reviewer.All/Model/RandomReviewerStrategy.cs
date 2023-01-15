namespace Inc.TeamAssistant.Reviewer.All.Model;

internal sealed class RandomReviewerStrategy : INextReviewerStrategy
{
    private static readonly Random RandomSelector = new();

    private readonly Team _team;
    private readonly int _minPlayersCount;

    public RandomReviewerStrategy(Team team, int minPlayersCount)
    {
        _team = team ?? throw new ArgumentNullException(nameof(team));
        _minPlayersCount = minPlayersCount;
    }
    
    public Player Next(Player owner)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));
        
        var excludedPlayers = owner.LastReviewerId.HasValue && _team.Players.Count > _minPlayersCount
            ? new[] { owner.Person.Id, owner.LastReviewerId.Value }
            : new[] { owner.Person.Id };

        var targetPlayers = _team.Players
            .Where(p => !excludedPlayers.Contains(p.Person.Id))
            .OrderBy(p => p.Id)
            .ToArray();
        var nextReviewerIndex = RandomSelector.Next(0, targetPlayers.Length);
        return targetPlayers[nextReviewerIndex];
    }
}