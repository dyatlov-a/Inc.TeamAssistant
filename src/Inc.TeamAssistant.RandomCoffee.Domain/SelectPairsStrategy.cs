namespace Inc.TeamAssistant.RandomCoffee.Domain;

internal sealed class SelectPairsStrategy
{
    private const int MaxSelectIterations = 10_000;
    private const int HistoryPairsWeight = 1;
    private const int NewPairsWeight = 10;
    private const int ExcludedPersonWeight = 20;
    
    private static readonly Random Random = new();
    
    private readonly long[] _orderedParticipantIds;
    private readonly PersonPair[][] _orderedHistory;

    public SelectPairsStrategy(IEnumerable<long> participantIds, PersonPair[][] orderedHistory)
    {
        ArgumentNullException.ThrowIfNull(participantIds);

        _orderedParticipantIds = participantIds.OrderBy(p => p).ToArray();
        _orderedHistory = orderedHistory ?? throw new ArgumentNullException(nameof(orderedHistory));
    }

    public (IReadOnlyCollection<PersonPair> Pairs, long? ExcludedPersonId) Detect(long? lastExcludedPersonId)
    {
        var pairCount = _orderedParticipantIds.Length / PersonPair.Size;
        var pairByWeights = SetPriorityForExcludedPerson(AddNewPairs(BuildByHistory(
            new Dictionary<PersonPair, int>())),
            lastExcludedPersonId);
        
        var pairs = new List<PersonPair>(pairCount);

        foreach (var _ in Enumerable.Range(0, MaxSelectIterations))
        {
            var targetPairByWeights = pairByWeights.Where(p => !p.Key.ContainsIn(pairs)).ToArray();
            if (targetPairByWeights.Length == 0)
                break;

            var pair = targetPairByWeights.Length == 1
                ? targetPairByWeights[0].Key
                : SelectFromSeeding(targetPairByWeights);
            pairs.Add(pair);
            
            if (pairs.Count == pairCount)
                break;
        }

        return (pairs, GetExcludedPerson(pairs));
    }

    private long? GetExcludedPerson(IReadOnlyList<PersonPair> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs);
        
        return _orderedParticipantIds.Length % PersonPair.Size != 0
            ? _orderedParticipantIds.Single(i => !pairs.Any(p => p.FirstId == i || p.SecondId == i))
            : null;
    }

    private PersonPair SelectFromSeeding(IReadOnlyCollection<KeyValuePair<PersonPair, int>> pairByWeights)
    {
        ArgumentNullException.ThrowIfNull(pairByWeights);
        
        var seedingLength = pairByWeights.Sum(p => p.Value);
        var seeding = Seeding(pairByWeights);
        var index = Random.Next(0, seedingLength - 1);
        return seeding.Skip(index).First();
    }

    private IEnumerable<PersonPair> Seeding(IReadOnlyCollection<KeyValuePair<PersonPair, int>> pairByWeights)
    {
        foreach (var pairByWeight in pairByWeights)
        foreach (var _ in Enumerable.Range(0, pairByWeight.Value))
            yield return pairByWeight.Key;
    }

    private Dictionary<PersonPair, int> BuildByHistory(Dictionary<PersonPair, int> pairByWeights)
    {
        ArgumentNullException.ThrowIfNull(pairByWeights);
        
        for (var i = 0; i < _orderedHistory.Length; i++)
        {
            var pairs = _orderedHistory[i]
                .Where(p => _orderedParticipantIds.Contains(p.FirstId) && _orderedParticipantIds.Contains(p.SecondId))
                .ToArray();
            var weight = (i + 1) * HistoryPairsWeight;
            
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
            pairByWeights.TryAdd(newPair, NewPairsWeight);
        
        return pairByWeights;
    }

    private Dictionary<PersonPair, int> SetPriorityForExcludedPerson(
        Dictionary<PersonPair, int> pairByWeights,
        long? lastExcludedPersonId)
    {
        ArgumentNullException.ThrowIfNull(pairByWeights);
        
        if (lastExcludedPersonId.HasValue)
        {
            var pair = pairByWeights.First(p => p.Key.HasPerson(lastExcludedPersonId.Value)).Key;
            pairByWeights[pair] = ExcludedPersonWeight;
        }
        
        return pairByWeights;
    }
}