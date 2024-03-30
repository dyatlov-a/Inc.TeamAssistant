using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.DomainTests.Extensions;

internal static class PersonPairExtensions
{
    public static bool IsIntersection(this IReadOnlyCollection<PersonPair> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs);

        var exists = new HashSet<long>();

        foreach (var pair in pairs)
        {
            if (!exists.Add(pair.FirstId))
                return true;
            if (!exists.Add(pair.SecondId))
                return true;
        }

        return false;
    }

    public static bool IsIntersection(
        this IReadOnlyCollection<PersonPair> pairs,
        IReadOnlyCollection<PersonPair> otherPairs)
    {
        ArgumentNullException.ThrowIfNull(pairs);
        ArgumentNullException.ThrowIfNull(otherPairs);

        return pairs.Any(p => otherPairs.Any(h => h.Equals(p)));
    }

    public static PersonPair[][] GetHistoryPart(this List<PersonPair[]> history, int count = 5)
    {
        ArgumentNullException.ThrowIfNull(history);
        
        return history
            .Skip(history.Count - count)
            .Take(count)
            .ToArray();
    }
}