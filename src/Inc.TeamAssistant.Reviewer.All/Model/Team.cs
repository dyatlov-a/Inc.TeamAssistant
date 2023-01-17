namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class Team
{
    public Guid Id { get; private set; }
    public long ChatId { get; private set; }
    public string Name { get; private set; } = default!;
    public NextReviewerType NextReviewerType { get; private set; }

    private readonly List<Player> _players = new();
    public IReadOnlyCollection<Player> Players => _players;

    private Team()
    {
    }

    public Team(long chatId, string name, NextReviewerType nextReviewerType)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        Id = Guid.NewGuid();
        ChatId = chatId;
        Name = name;
        NextReviewerType = nextReviewerType;
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

    public bool CanStartReview() => _players.Any();

    public Player GetNextReviewer(Person owner, Person? lastReviewer = null)
        => NextReviewerStrategy.Next(owner, lastReviewer);

    internal INextReviewerStrategy NextReviewerStrategy => NextReviewerType switch
    {
        NextReviewerType.RoundRobin => new RoundRobinReviewerStrategy(this),
        NextReviewerType.Random => new RandomReviewerStrategy(this),
        _ => throw new ApplicationException($"NextReviewerType for team {Id} was not valid.")
    };
}