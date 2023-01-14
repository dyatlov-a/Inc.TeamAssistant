using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class Team
{
    private const int MinPlayersCount = 2;
    
    public Guid Id { get; private set; }
    public long ChatId { get; private set; }
    public string Name { get; private set; } = default!;

    private readonly List<Player> _players = new();
    public IReadOnlyCollection<Player> Players => _players;

    private Team()
    {
    }

    public Team(long chatId, string name)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        Id = Guid.NewGuid();
        ChatId = chatId;
        Name = name;
    }

    public void AddPlayer(LanguageId languageId, long userId, string name, string? login)
    {
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (_players.Any(p => p.UserId == userId))
            throw new ApplicationException($"User {name} already exists in team {Name}.");

        _players.Add(new(languageId, userId, Id, name, login));
    }

    public Team Build(IReadOnlyCollection<Player> players)
    {
        if (players is null)
            throw new ArgumentNullException(nameof(players));
        if (_players.Any())
            throw new ApplicationException("Map failed. Team already has players.");
        if (players.Any(p => p.TeamId != Id))
            throw new ApplicationException("Map failed. Players from other team.");

        foreach (var player in players)
            _players.Add(player);

        return this;
    }

    public bool CanStartReview() => _players.Count >= MinPlayersCount;

    public TaskForReview CreateTaskForReview(long playerId, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));
        if (_players.Count < MinPlayersCount)
            throw new ApplicationException($"Team has not {MinPlayersCount} players.");

        var player = _players.Single(p => p.UserId == playerId);
        var lastReviewerId = player.LastReviewerId ?? long.MaxValue;

        var otherPlayers = _players.Where(p => p.UserId != player.UserId).ToArray();
        var nextReviewer = otherPlayers.Where(p => p.UserId > lastReviewerId).MinBy(p => p.UserId)
            ?? otherPlayers.MinBy(p => p.UserId)!;

        var owner = new PlayerAsOwner(player, nextReviewer.UserId);
        var reviewer = new PlayerAsReviewer(nextReviewer);
        return new(owner, reviewer, ChatId, description);
    }
}