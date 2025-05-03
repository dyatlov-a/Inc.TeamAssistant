namespace Inc.TeamAssistant.RandomCoffee.Domain;

public sealed class RandomCoffeeHistory
{
    public Guid Id { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public Guid RandomCoffeeEntryId { get; private set; }
    public IReadOnlyCollection<PersonPair> Pairs { get; private set; } = [];
    public long? ExcludedPersonId { get; private set; }

    private RandomCoffeeHistory()
    {
    }

    internal RandomCoffeeHistory(
        Guid id,
        DateTimeOffset now,
        Guid randomCoffeeEntryId,
        IReadOnlyCollection<PersonPair> pairs,
        long? excludedPersonId)
        : this()
    {
        Id = id;
        Created = now;
        RandomCoffeeEntryId = randomCoffeeEntryId;
        Pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
        ExcludedPersonId = excludedPersonId;
    }
}