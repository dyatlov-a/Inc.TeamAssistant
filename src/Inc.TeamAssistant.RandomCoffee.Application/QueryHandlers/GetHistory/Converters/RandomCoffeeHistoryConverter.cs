using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

namespace Inc.TeamAssistant.RandomCoffee.Application.QueryHandlers.GetHistory.Converters;

internal sealed class RandomCoffeeHistoryConverter
{
    private readonly ITeamAccessor _teamAccessor;

    public RandomCoffeeHistoryConverter(ITeamAccessor teamAccessor)
    {
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<IReadOnlyCollection<RandomCoffeeHistoryDto>> ConvertTo(
        IReadOnlyCollection<RandomCoffeeHistory> items,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(items);
        
        var personLookup = await BuildLookup(items, token);
        var results = items
            .Select(i => ConvertTo(i, personLookup))
            .ToArray();

        return results;
    }

    private RandomCoffeeHistoryDto ConvertTo(
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
    
    private async Task<IReadOnlyDictionary<long, Person>> BuildLookup(
        IReadOnlyCollection<RandomCoffeeHistory> items,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(items);
        
        var personIds = items
            .SelectMany(i => i.Pairs.Select(p => p.FirstId).Union(i.Pairs.Select(p => p.SecondId)))
            .Union(items.Where(h => h.ExcludedPersonId.HasValue).Select(h => h.ExcludedPersonId!.Value))
            .Distinct();
        
        var lookup = new Dictionary<long, Person>();

        foreach (var personId in personIds)
        {
            var person = await _teamAccessor.FindPerson(personId, token);

            lookup.Add(personId, person ?? new Person(personId, personId.ToString(), Username: null));
        }

        return lookup;
    }
}