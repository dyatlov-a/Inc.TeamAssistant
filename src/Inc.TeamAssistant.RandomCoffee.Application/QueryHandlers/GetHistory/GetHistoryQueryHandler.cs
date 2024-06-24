using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
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

        var personIds = historyItems
            .SelectMany(i => i.Pairs.Select(p => p.FirstId).Union(i.Pairs.Select(p => p.SecondId)))
            .Union(historyItems.Where(h => h.ExcludedPersonId.HasValue).Select(h => h.ExcludedPersonId!.Value))
            .Distinct()
            .ToArray();
        var personLookup = new Dictionary<long, string>();

        foreach (var personId in personIds)
        {
            var person = await _teamAccessor.FindPerson(personId, token);

            personLookup.Add(personId, person?.DisplayName ?? personId.ToString());
        }

        var results = historyItems
            .Select(i => new RandomCoffeeHistoryDto(
                new DateOnly(i.Created.Year, i.Created.Month, i.Created.Day),
                i.Pairs.Select(p => new PairDto(personLookup[p.FirstId], personLookup[p.SecondId])).ToArray(),
                i.ExcludedPersonId.HasValue ? personLookup[i.ExcludedPersonId.Value] : null))
            .ToArray();

        return new GetHistoryResult(results);
    }
}