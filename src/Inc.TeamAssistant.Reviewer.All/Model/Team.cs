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

    public void AddPlayer(Person person)
    {
        if (person is null)
            throw new ArgumentNullException(nameof(person));
        if (_players.Any(p => p.Person.Id == person.Id))
            throw new ApplicationException($"User {person} already exists in team {Name}.");

        _players.Add(new(person, Id));
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

    public TaskForReview CreateTaskForReview(long userId, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));
        if (_players.Count < MinPlayersCount)
            throw new ApplicationException($"Team has not {MinPlayersCount} players.");

        var player = _players.Single(p => p.Person.Id == userId);
        var lastReviewerId = player.LastReviewerId ?? long.MaxValue;

        var otherPlayers = _players.Where(p => p.Person.Id != player.Person.Id).ToArray();
        var nextReviewer = otherPlayers.Where(p => p.Person.Id > lastReviewerId).MinBy(p => p.Person.Id)
            ?? otherPlayers.MinBy(p => p.Person.Id)!;

        var owner = new PlayerAsOwner(player, nextReviewer.Person.Id);
        var reviewer = new PlayerAsReviewer(nextReviewer);
        return new(owner, reviewer, ChatId, description);
    }
}