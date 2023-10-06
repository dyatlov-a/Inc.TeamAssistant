namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

internal sealed class RandomReviewerStrategy : INextReviewerStrategy
{
    private static readonly Random RandomSelector = new();

    private readonly Team _team;

    public RandomReviewerStrategy(Team team)
    {
        _team = team ?? throw new ArgumentNullException(nameof(team));
    }
    
    public Person Next(Person owner, Person? lastReviewer)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));

        var excludedPlayers = lastReviewer is null ? new[] { owner.Id } : new[] { owner.Id, lastReviewer.Id };
        var targetPlayers = _team.Players
            .Where(p => !excludedPlayers.Contains(p.Id))
            .OrderBy(p => p.Id)
            .ToArray();

        if (!targetPlayers.Any())
            return _team.Players.First();
        
        var nextReviewerIndex = RandomSelector.Next(0, targetPlayers.Length);
        return targetPlayers[nextReviewerIndex];
    }
}