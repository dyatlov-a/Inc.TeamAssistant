namespace Inc.TeamAssistant.RandomCoffee.Domain;

public sealed class RandomCoffeeHistory
{
    public Guid Id { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public Guid RandomCoffeeEntryId { get; private set; }
    public ICollection<PersonPair> Pairs { get; private set; }
    public long? ExcludedPersonId { get; private set; }

    private RandomCoffeeHistory()
    {
        Pairs = new List<PersonPair>();
    }

    private RandomCoffeeHistory(Guid id, DateTimeOffset now, Guid randomCoffeeEntryId)
        : this()
    {
        Id = id;
        Created = now;
        RandomCoffeeEntryId = randomCoffeeEntryId;
    }
    
    public static RandomCoffeeHistory Build(Guid entryId, IReadOnlyCollection<PersonPair> pairs, long? excludedPersonId)
    {
        ArgumentNullException.ThrowIfNull(pairs);

        var randomCoffeeHistory = new RandomCoffeeHistory(Guid.NewGuid(), DateTimeOffset.UtcNow, entryId);
        
        foreach (var pair in pairs)
            randomCoffeeHistory.Pairs.Add(pair);
        
        if (excludedPersonId.HasValue)
            randomCoffeeHistory.ExcludedPersonId = excludedPersonId.Value;

        return randomCoffeeHistory;
    }
}