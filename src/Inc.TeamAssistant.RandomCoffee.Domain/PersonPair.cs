namespace Inc.TeamAssistant.RandomCoffee.Domain;

public sealed record PersonPair(long FirstId, long SecondId)
{
    public bool IsEquivalent(PersonPair otherPair)
    {
        if (otherPair is null)
            throw new ArgumentNullException(nameof(otherPair));
        
        if (FirstId == otherPair.FirstId && SecondId == otherPair.SecondId)
            return true;
        if (FirstId == otherPair.SecondId && SecondId == otherPair.FirstId)
            return true;

        return false;
    }
    
    public bool Intersect(IReadOnlyCollection<PersonPair> pairs)
    {
        if (pairs is null)
            throw new ArgumentNullException(nameof(pairs));
        
        var exists = new HashSet<long>();

        foreach (var item in pairs)
        {
            exists.Add(item.FirstId);
            exists.Add(item.SecondId);
        }
        
        return exists.Contains(FirstId) || exists.Contains(SecondId);
    }
}