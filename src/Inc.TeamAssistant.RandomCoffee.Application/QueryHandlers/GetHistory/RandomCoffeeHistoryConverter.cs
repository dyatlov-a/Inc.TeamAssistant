using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

namespace Inc.TeamAssistant.RandomCoffee.Application.QueryHandlers.GetHistory;

internal static class RandomCoffeeHistoryConverter
{
    public static RandomCoffeeHistoryDto ConvertTo(
        RandomCoffeeHistory randomCoffeeHistory,
        IReadOnlyDictionary<long, Person> personLookup)
    {
        ArgumentNullException.ThrowIfNull(randomCoffeeHistory);
        ArgumentNullException.ThrowIfNull(personLookup);

        var excludedPerson = randomCoffeeHistory.ExcludedPersonId.HasValue
            ? personLookup[randomCoffeeHistory.ExcludedPersonId.Value]
            : null;
        var createdDate = new DateOnly(
            randomCoffeeHistory.Created.Year,
            randomCoffeeHistory.Created.Month,
            randomCoffeeHistory.Created.Day);
        var pairs = randomCoffeeHistory.Pairs
            .Select(p =>
            {
                var firstPerson = personLookup[p.FirstId];
                var secondPerson = personLookup[p.SecondId];
                return new PairDto(
                    firstPerson.Name,
                    firstPerson.Username,
                    secondPerson.Name,
                    secondPerson.Username);
            })
            .ToArray();
        
        return new RandomCoffeeHistoryDto(
            createdDate,
            pairs,
            excludedPerson?.Name,
            excludedPerson?.Username);
    }
}