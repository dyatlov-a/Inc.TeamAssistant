namespace Inc.TeamAssistant.Primitives.Features.PersonStats;

public interface IPersonStatsProvider
{
    string FeatureName { get; }
    
    Task<IReadOnlyDictionary<long, int>> GetStats(
        IReadOnlyCollection<long> personIds,
        DateTimeOffset from,
        CancellationToken token);
}