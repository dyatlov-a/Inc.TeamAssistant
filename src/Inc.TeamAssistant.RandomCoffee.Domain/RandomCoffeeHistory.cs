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

    private RandomCoffeeHistory(Guid randomCoffeeEntryId)
        : this()
    {
        Id = Guid.NewGuid();
        Created = DateTimeOffset.UtcNow;
        RandomCoffeeEntryId = randomCoffeeEntryId;
    }
    
    public static RandomCoffeeHistory Build(Guid entryId, IReadOnlyCollection<PersonPair> pairs, long? excludedPersonId)
    {
        if (pairs is null)
            throw new ArgumentNullException(nameof(pairs));
        
        var randomCoffeeHistory = new RandomCoffeeHistory(entryId);
        
        foreach (var pair in pairs)
            randomCoffeeHistory.Pairs.Add(pair);
        
        if (excludedPersonId.HasValue)
            randomCoffeeHistory.ExcludedPersonId = excludedPersonId.Value;

        return randomCoffeeHistory;
    }
}