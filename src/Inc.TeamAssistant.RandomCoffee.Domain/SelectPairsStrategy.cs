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
            
        var seeding = new List<PersonPair>(pairByWeights.Values.Sum(v => v));
        var pairs = new List<PersonPair>(pairCount);
        
        foreach (var pairByWeight in pairByWeights)
        foreach (var _ in Enumerable.Range(0, pairByWeight.Value))
            seeding.Add(pairByWeight.Key);

        foreach (var _ in Enumerable.Range(0, MaxSelectIterations))
        {
            var index = Random.Next(0, seeding.Count - 1);
            var pair = seeding[index];
        
            if (!pair.ContainsIn(pairs))
                pairs.Add(pair);

            if (pairs.Count != pairCount)
            {
                var newPairs = seeding.Where(p => !p.ContainsIn(pairs)).Distinct().ToArray();
                if (newPairs.Length == 1)
                    pairs.Add(newPairs.Single());
            }
            
            if (pairs.Count == pairCount)
                break;
        }

        var excludedPersonId = _orderedParticipantIds.Length % PersonPair.Size != 0
            ? _orderedParticipantIds.Single(i => !pairs.Any(p => p.FirstId == i || p.SecondId == i))
            : (long?)null;

        return (pairs, excludedPersonId);
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