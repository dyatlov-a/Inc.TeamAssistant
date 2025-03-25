using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.RandomCoffee.Domain;

public sealed class RandomCoffeeEntry
{
    public Guid Id { get; private set; }
    public Guid BotId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public long ChatId { get; private set; }
    public string Name { get; private set; } = default!;
    public long OwnerId { get; private set; }
    public DateTimeOffset NextRound { get; private set; }
    public RandomCoffeeState State { get; private set; }
    public string? PollId { get; private set; }
    public ICollection<long> ParticipantIds { get; private set; }

    private readonly List<RandomCoffeeHistory> _history = new();
    public IReadOnlyCollection<RandomCoffeeHistory> History => _history;
    
    public bool Refused { get; private set; }

    private RandomCoffeeEntry()
    {
        ParticipantIds = new List<long>();
    }

    public RandomCoffeeEntry(Guid id, Guid botId, long chatId, string name, long ownerId)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Created = DateTimeOffset.UtcNow;
        BotId = botId;
        ChatId = chatId;
        Name = name;
        OwnerId = ownerId;
    }

    public RandomCoffeeEntry AddHistory(RandomCoffeeHistory randomCoffeeHistory)
    {
        ArgumentNullException.ThrowIfNull(randomCoffeeHistory);

        _history.Add(randomCoffeeHistory);

        return this;
    }

    public RandomCoffeeEntry MoveToWaiting(DateTimeOffset now, TimeSpan waitingInterval, long? personId)
    {
        var entry = personId.HasValue ? EnsureRights(personId.Value) : this;
        
        entry.NextRound = now.Add(waitingInterval);
        entry.State = RandomCoffeeState.Waiting;
        entry.ParticipantIds.Clear();
        entry.PollId = null;
        entry.Refused = false;
        
        return entry;
    }

    public RandomCoffeeEntry MoveToNextRound(DateTimeOffset now, TimeSpan roundInterval, TimeSpan votingInterval)
    {
        var waitingInterval = roundInterval - votingInterval;
        
        NextRound = now.Add(waitingInterval);
        State = RandomCoffeeState.Idle;

        return this;
    }
    
    public RandomCoffeeEntry AddPerson(long participantId)
    {
        if (State == RandomCoffeeState.Waiting && !ParticipantIds.Contains(participantId))
            ParticipantIds.Add(participantId);

        return this;
    }

    public RandomCoffeeEntry RemovePerson(long participantId)
    {
        if (State == RandomCoffeeState.Waiting && ParticipantIds.Contains(participantId))
            ParticipantIds.Remove(participantId);

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
        var lastExcludedPersonId = History.MaxBy(i => i.Created)?.ExcludedPersonId;
        var strategy = new SelectPairsStrategy(ParticipantIds, orderedHistory);
        var detectedPairs = strategy.Detect(lastExcludedPersonId);
        var randomCoffeeHistory = RandomCoffeeHistory.Build(Id, detectedPairs.Pairs, detectedPairs.ExcludedPersonId);
        
        _history.Add(randomCoffeeHistory);
        
        return randomCoffeeHistory;
    }

    public RandomCoffeeEntry MoveToRefused(long personId)
    {
        var entry = EnsureRights(personId);
        
        entry.Refused = true;
        
        return entry;
    }
    
    private RandomCoffeeEntry EnsureRights(long personId)
    {
        if (OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);

        return this;
    }
}