namespace Inc.TeamAssistant.RandomCoffee.Domain;

internal sealed class SelectPairsStrategy
{
    public const int PairSize = 2;

    private readonly long[] _orderedParticipantIds;
    private readonly PersonPair[][] _orderedHistory;

    public SelectPairsStrategy(IEnumerable<long> participantIds, PersonPair[][] orderedHistory)
    {
        if (participantIds is null)
            throw new ArgumentNullException(nameof(participantIds));
        
        _orderedParticipantIds = participantIds.OrderBy(p => p).ToArray();
        _orderedHistory = orderedHistory ?? throw new ArgumentNullException(nameof(orderedHistory));
    }

    public (IReadOnlyCollection<PersonPair> Pairs, long? ExcludedPersonId) Detect()
    {
        var pairCount = _orderedParticipantIds.Length / PairSize;
        var pairByWeights = AddNewPairs(BuildByHistory());
        
        var seeding = new List<PersonPair>();

        foreach (var pairByWeight in pairByWeights)
            for (var index = 0; index < pairByWeight.Weight; index++)
                seeding.Add(pairByWeight.Pair);

        var random = new Random();
        var pairs = new List<PersonPair>();

        do
        {
            var index = random.Next(0, seeding.Count - 1);
            var pair = seeding[index];
        
            if (!pair.Intersect(pairs))
                pairs.Add(pair);
        } while (pairs.Count < pairCount);

        var excludedPersonId = _orderedParticipantIds.Length % PairSize != 0
            ? _orderedParticipantIds.Single(i => !pairs.Any(p => p.FirstId == i || p.SecondId == i))
            : (long?)null;

        return (pairs, excludedPersonId);
    }

    private List<(PersonPair Pair, int Weight)> BuildByHistory()
    {
        var pairByWeights = new List<(PersonPair Pair, int Weight)>();

        for (var index = 0; index < _orderedHistory.Length; index++)
        {
            var pairs = _orderedHistory[index]
                .Where(p => _orderedParticipantIds.Contains(p.FirstId) && _orderedParticipantIds.Contains(p.SecondId))
                .ToArray();
            
            foreach (var pair in pairs)
                pairByWeights.Add((pair, index));
        }

        return pairByWeights;
    }

    private List<(PersonPair Pair, int Weight)> AddNewPairs(List<(PersonPair Pair, int Weight)> pairByWeights)
    {
        if (pairByWeights is null)
            throw new ArgumentNullException(nameof(pairByWeights));
        
        var newPairs = new List<PersonPair>();

        for (var i = 0; i < _orderedParticipantIds.Length; i++)
        for (var j = i + 1; j < _orderedParticipantIds.Length; j++)
        {
            var firstParticipantId = _orderedParticipantIds[i];
            var secondParticipantId = _orderedParticipantIds[j];

            newPairs.Add(new(firstParticipantId, secondParticipantId));
        }

        foreach (var newPair in newPairs)
            if (!pairByWeights.Any(i => i.Pair.IsEquivalent(newPair)))
                pairByWeights.Add((newPair, (_orderedHistory.Length + 1) * PairSize));
        
        return pairByWeights;
    }
}