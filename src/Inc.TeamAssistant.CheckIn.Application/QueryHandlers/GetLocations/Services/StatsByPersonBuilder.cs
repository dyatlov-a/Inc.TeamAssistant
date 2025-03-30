using Inc.TeamAssistant.Primitives.Features.PersonStats;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;

internal sealed class StatsByPersonBuilder
{
    private readonly IEnumerable<IPersonStatsProvider> _providers;

    public StatsByPersonBuilder(IEnumerable<IPersonStatsProvider> providers)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyDictionary<long, int>>> Build(
        IReadOnlyCollection<long> personIds,
        DateTimeOffset from,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(personIds);
        
        var lookup = new Dictionary<string, IReadOnlyDictionary<long, int>>();

        foreach (var personStatsProvider in _providers)
            lookup.Add(personStatsProvider.FeatureName, await personStatsProvider.GetStats(personIds, from, token));

        return lookup;
    }
}