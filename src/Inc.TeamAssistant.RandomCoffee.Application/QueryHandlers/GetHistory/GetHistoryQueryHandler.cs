using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.QueryHandlers.GetHistory;

internal sealed class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, GetHistoryResult>
{
    private readonly IRandomCoffeeReader _reader;
    private readonly ITeamAccessor _teamAccessor;

    public GetHistoryQueryHandler(IRandomCoffeeReader reader, ITeamAccessor teamAccessor)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<GetHistoryResult> Handle(GetHistoryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var historyItems = await _reader.GetHistory(query.BotId, query.ChatId, query.Depth, token);
        var personLookup = await BuildPersonLookup(historyItems, token);
        var results = historyItems
            .Select(i => RandomCoffeeHistoryConverter.ConvertTo(i, personLookup))
            .ToArray();
        
        return new GetHistoryResult(results);
    }

    private async Task<IReadOnlyDictionary<long, Person>> BuildPersonLookup(
        IReadOnlyCollection<RandomCoffeeHistory> historyItems,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(historyItems);
        
        var personIds = historyItems
            .SelectMany(i => i.Pairs.Select(p => p.FirstId).Union(i.Pairs.Select(p => p.SecondId)))
            .Union(historyItems.Where(h => h.ExcludedPersonId.HasValue).Select(h => h.ExcludedPersonId!.Value))
            .Distinct();
        
        var personLookup = new Dictionary<long, Person>();

        foreach (var personId in personIds)
        {
            var person = await _teamAccessor.FindPerson(personId, token);

            personLookup.Add(personId, person ?? new Person(personId, personId.ToString(), Username: null));
        }

        return personLookup;
    }
}