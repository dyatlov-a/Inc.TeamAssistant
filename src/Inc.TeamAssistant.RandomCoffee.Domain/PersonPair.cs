namespace Inc.TeamAssistant.RandomCoffee.Domain;

public sealed record PersonPair
{
    public const int Size = 2;
    
    public long FirstId { get; }
    public long SecondId { get; }

    public PersonPair(long firstId, long secondId)
    {
        if (firstId == secondId)
            throw new ArgumentException($"FirstId {firstId} AND SecondId {secondId} can not be equals.");

        if (firstId < secondId)
        {
            FirstId = firstId;
            SecondId = secondId;
        }
        else
        {
            FirstId = secondId;
            SecondId = firstId;
        }
    }

    public bool ContainsIn(IReadOnlyCollection<PersonPair> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs);

        var exists = new HashSet<long>();

        foreach (var item in pairs)
        {
            exists.Add(item.FirstId);
            exists.Add(item.SecondId);
        }
        
        return exists.Contains(FirstId) || exists.Contains(SecondId);
    }

    public bool HasPerson(long personId) => FirstId == personId || SecondId == personId;
}