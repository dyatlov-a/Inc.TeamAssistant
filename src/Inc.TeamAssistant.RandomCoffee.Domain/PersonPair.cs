namespace Inc.TeamAssistant.RandomCoffee.Domain;

public sealed class PersonPair
{
    public const int Size = 2;
    
    public long FirstId { get; private set; }
    public long SecondId { get; private set; }

    private PersonPair()
    {
    }

    public PersonPair(long firstId, long secondId)
        : this()
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

    public override bool Equals(object? obj)
    {
        var other = obj as PersonPair;

        return other is not null && Equals(other);
    }

    private bool Equals(PersonPair other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return FirstId == other.FirstId && SecondId == other.SecondId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FirstId, SecondId);
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