namespace Inc.TeamAssistant.RandomCoffee.Domain;

internal sealed class SelectPairsStrategy
{
    private static readonly Random Random = new();
    
    private readonly long[] _orderedParticipantIds;
    private readonly PersonPair[][] _orderedHistory;

    public SelectPairsStrategy(IEnumerable<long> participantIds, PersonPair[][] orderedHistory)
    {
        ArgumentNullException.ThrowIfNull(participantIds);

        _orderedParticipantIds = participantIds.OrderBy(p => p).ToArray();
        _orderedHistory = orderedHistory ?? throw new ArgumentNullException(nameof(orderedHistory));
    }

    public (IReadOnlyCollection<PersonPair> Pairs, long? ExcludedPersonId) Detect()
    {
        var pairCount = _orderedParticipantIds.Length / PersonPair.Size;
        var pairByWeights = AddNewPairs(BuildByHistory());
        var seeding = new List<PersonPair>(pairByWeights.Values.Sum(v => v));
        var pairs = new List<PersonPair>(pairCount);
        
        foreach (var pairByWeight in pairByWeights)
        foreach (var _ in Enumerable.Range(0, pairByWeight.Value))
            seeding.Add(pairByWeight.Key);

        foreach (var _ in Enumerable.Range(0, int.MaxValue))
        {
            var index = Random.Next(0, seeding.Count - 1);
            var pair = seeding[index];
        
            if (!pair.ContainsIn(pairs))
                pairs.Add(pair);
            
            if (pairs.Count == pairCount)
                break;
        }

        var excludedPersonId = _orderedParticipantIds.Length % PersonPair.Size != 0
            ? _orderedParticipantIds.Single(i => !pairs.Any(p => p.FirstId == i || p.SecondId == i))
            : (long?)null;

        return (pairs, excludedPersonId);
    }

    private Dictionary<PersonPair, int> BuildByHistory()
    {
        var pairByWeights = new Dictionary<PersonPair, int>();

        for (var index = 0; index < _orderedHistory.Length; index++)
        {
            var pairs = _orderedHistory[index]
                .Where(p => _orderedParticipantIds.Contains(p.FirstId) && _orderedParticipantIds.Contains(p.SecondId))
                .ToArray();
            var weight = _orderedParticipantIds.Length < PersonPair.Size * 2
                ? index + 1
                : index;
            
            foreach (var pair in pairs)
                pairByWeights.TryAdd(pair, weight);
        }

        return pairByWeights;
    }

    private Dictionary<PersonPair, int> AddNewPairs(Dictionary<PersonPair, int> pairByWeights)
    {
        ArgumentNullException.ThrowIfNull(pairByWeights);

        var newPairs = new List<PersonPair>();

        for (var i = 0; i < _orderedParticipantIds.Length; i++)
        for (var j = i + 1; j < _orderedParticipantIds.Length; j++)
        {
            var firstParticipantId = _orderedParticipantIds[i];
            var secondParticipantId = _orderedParticipantIds[j];

            newPairs.Add(new(firstParticipantId, secondParticipantId));
        }

        foreach (var newPair in newPairs)
            pairByWeights.TryAdd(newPair, (_orderedHistory.Length + 1) * PersonPair.Size);
        
        return pairByWeights;
    }
}