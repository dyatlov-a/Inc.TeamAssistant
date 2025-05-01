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
    public PollEntry? Poll { get; private set; }
    public ICollection<long> ParticipantIds { get; private set; }

    private readonly List<RandomCoffeeHistory> _history = new();
    public IReadOnlyCollection<RandomCoffeeHistory> History => _history;

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
        
        return entry;
    }

    public int? MoveToNextRound(DateTimeOffset now, TimeSpan roundInterval, TimeSpan votingInterval)
    {
        var waitingInterval = roundInterval - votingInterval;
        var pollMessageId = Poll?.MessageId;
        
        NextRound = now.Add(waitingInterval);
        State = RandomCoffeeState.Idle;
        Poll = null;

        return pollMessageId;
    }
    
    public RandomCoffeeEntry SetAnswer(bool isAttend, long participantId)
    {
        var entry = State == RandomCoffeeState.Waiting
            ? isAttend
                ? AddPerson(participantId)
                : RemovePerson(participantId)
            : this;

        return entry;
    }

    public RandomCoffeeEntry AttachPoll(string pollId, int messageId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pollId);
        
        Poll = new PollEntry(pollId, messageId);
        
        return this;
    }

    public RandomCoffeeHistory? TrySelectPairs()
    {
        if (ParticipantIds.Count < PersonPair.Size)
            return null;
        
        var orderedHistory = History
            .OrderByDescending(i => i.Created)
            .Select(i => i.Pairs.ToArray())
            .ToArray();
        var lastExcludedPersonId = History.MaxBy(i => i.Created)?.ExcludedPersonId;
        var strategy = new SelectPairsStrategy(ParticipantIds, orderedHistory);
        var detectedPairs = strategy.Detect(lastExcludedPersonId);
        var randomCoffeeHistory = RandomCoffeeHistory.Build(
            Id,
            detectedPairs.Pairs,
            detectedPairs.ExcludedPersonId);
        
        _history.Add(randomCoffeeHistory);

        return randomCoffeeHistory;
    }

    public int? MoveToRefused(long personId)
    {
        EnsureRights(personId);
        
        var messageId = Poll?.MessageId;
        
        Poll = null;
        State = RandomCoffeeState.Refused;
        
        return messageId;
    }

    public bool AlreadyStarted(bool onDemand)
    {
        const int hasOnlyOneCondition = 1;
        
        var isRefused = State == RandomCoffeeState.Refused;
        var startConditions = new[] { isRefused, onDemand };
        
        return startConditions.Count(i => i) == hasOnlyOneCondition;
    }
    
    private RandomCoffeeEntry AddPerson(long participantId)
    {
        if (!ParticipantIds.Contains(participantId))
            ParticipantIds.Add(participantId);

        return this;
    }

    private RandomCoffeeEntry RemovePerson(long participantId)
    {
        if (ParticipantIds.Contains(participantId))
            ParticipantIds.Remove(participantId);

        return this;
    }
    
    private RandomCoffeeEntry EnsureRights(long personId)
    {
        if (OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);

        return this;
    }
}