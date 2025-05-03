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
    public IReadOnlyCollection<long> ParticipantIds { get; private set; } = [];

    private readonly List<RandomCoffeeHistory> _history = new();
    public IReadOnlyCollection<RandomCoffeeHistory> History => _history;

    private RandomCoffeeEntry()
    {
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
    
    public bool IsWaitAnswer() => State == RandomCoffeeState.Waiting;
    public bool IsRefused() => State == RandomCoffeeState.Refused;
    
    public RandomCoffeeEntry CheckRights(long personId)
    {
        if (OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);

        return this;
    }

    public RandomCoffeeEntry AddHistory(RandomCoffeeHistory randomCoffeeHistory)
    {
        ArgumentNullException.ThrowIfNull(randomCoffeeHistory);

        _history.Add(randomCoffeeHistory);

        return this;
    }

    public RandomCoffeeEntry MoveToWaiting(DateTimeOffset now, TimeSpan waitingInterval)
    {
        if (IsWaitAnswer())
            throw new TeamAssistantUserException(Messages.RandomCoffee_AlreadyWaitAnswer);
        
        NextRound = now.Add(waitingInterval);
        State = RandomCoffeeState.Waiting;
        ParticipantIds = [];
        Poll = null;
        
        return this;
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
        var entry = isAttend
            ? AddPerson(participantId)
            : RemovePerson(participantId);

        return entry;
    }

    public RandomCoffeeEntry AttachPoll(string pollId, int messageId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pollId);
        
        Poll = new PollEntry(pollId, messageId);
        
        return this;
    }

    public RandomCoffeeHistory? TrySelectPairs(DateTimeOffset now)
    {
        if (ParticipantIds.Count < PersonPair.Size)
            return null;
        
        var orderedHistory = History
            .OrderByDescending(i => i.Created)
            .Select(i => i.Pairs.ToArray())
            .ToArray();
        var lastExcludedPersonId = History.MaxBy(i => i.Created)?.ExcludedPersonId;
        var detectedPairs = new SelectPairsStrategy(ParticipantIds, orderedHistory).Detect(lastExcludedPersonId);
        var randomCoffeeHistory = new RandomCoffeeHistory(
            Guid.NewGuid(),
            now,
            Id,
            detectedPairs.Pairs,
            detectedPairs.ExcludedPersonId);
        
        _history.Add(randomCoffeeHistory);
        
        return randomCoffeeHistory;
    }

    public int? MoveToRefused()
    {
        var messageId = Poll?.MessageId;
        
        Poll = null;
        State = RandomCoffeeState.Refused;
        
        return messageId;
    }
    
    private RandomCoffeeEntry AddPerson(long participantId)
    {
        ParticipantIds = ParticipantIds
            .Append(participantId)
            .Distinct()
            .ToArray();

        return this;
    }

    private RandomCoffeeEntry RemovePerson(long participantId)
    {
        ParticipantIds = ParticipantIds
            .Where(p => p != participantId)
            .ToArray();

        return this;
    }
}