namespace Inc.TeamAssistant.RandomCoffee.Domain;

public sealed class RandomCoffeeEntry
{
    public Guid Id { get; private set; }
    public Guid BotId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public long ChatId { get; private set; }
    public long OwnerId { get; private set; }
    public DateOnly NextRound { get; private set; }
    public RandomCoffeeState State { get; private set; }
    public string? PollId { get; private set; }
    public ICollection<long> ParticipantIds { get; private set; }

    private readonly List<RandomCoffeeHistory> _history = new();
    public IReadOnlyCollection<RandomCoffeeHistory> History => _history;

    private RandomCoffeeEntry()
    {
        ParticipantIds = new List<long>();
    }

    public RandomCoffeeEntry(Guid botId, long chatId, long ownerId)
        : this()
    {
        Id = Guid.NewGuid();
        Created = DateTimeOffset.UtcNow;
        BotId = botId;
        ChatId = chatId;
        OwnerId = ownerId;
    }

    public RandomCoffeeEntry AddHistory(RandomCoffeeHistory randomCoffeeHistory)
    {
        if (randomCoffeeHistory is null)
            throw new ArgumentNullException(nameof(randomCoffeeHistory));
        
        _history.Add(randomCoffeeHistory);

        return this;
    }

    public RandomCoffeeEntry MoveToWaiting(TimeSpan waitingInterval)
    {
        NextRound = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Add(waitingInterval).Date);
        State = RandomCoffeeState.Waiting;
        
        ParticipantIds.Clear();
        PollId = null;
        
        return this;
    }

    public RandomCoffeeEntry MoveToNextRound(TimeSpan roundInterval)
    {
        NextRound = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Add(roundInterval).Date);
        State = RandomCoffeeState.Idle;

        return this;
    }
    
    public RandomCoffeeEntry AddPerson(long participantId)
    {
        ParticipantIds.Add(participantId);

        return this;
    }

    public RandomCoffeeEntry AttachPoll(string pollId)
    {
        PollId = pollId;
        
        return this;
    }

    public bool CanSelectPairs() => ParticipantIds.Count >= PersonPair.Size;

    public RandomCoffeeHistory SelectPairs()
    {
        var orderedHistory = History
            .OrderByDescending(i => i.Created)
            .Select(i => i.Pairs.ToArray())
            .ToArray();
        
        var result = new SelectPairsStrategy(ParticipantIds, orderedHistory).Detect();
        
        var randomCoffeeHistory = RandomCoffeeHistory.Build(Id, result.Pairs, result.ExcludedPersonId);
        _history.Add(randomCoffeeHistory);
        return randomCoffeeHistory;
    }
}