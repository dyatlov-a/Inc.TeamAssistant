using AutoFixture;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Xunit;

namespace Inc.TeamAssistant.RandomCoffee.DomainTests;

public sealed class SelectPairsStrategyTests
{
    private readonly Fixture _fixture = new();
    
    [Fact]
    public void Detect_ParticipantIds_ShouldBeSelectPairs()
    {
        var orderedHistory = Array.Empty<PersonPair[]>();
        var participantIds = new[]
        {
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        };
        
        var strategy = new SelectPairsStrategy(participantIds, orderedHistory);

        var result = strategy.Detect();
        
        Assert.False(IsIntersection(result.Pairs));
        Assert.Null(result.ExcludedPersonId);
    }
    
    [Fact]
    public void Detect_ParticipantIds_ShouldBeSelectPairsWithExcluded()
    {
        var orderedHistory = Array.Empty<PersonPair[]>();
        var participantIds = new[]
        {
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        };
        
        var strategy = new SelectPairsStrategy(participantIds, orderedHistory);

        var result = strategy.Detect();
        
        var pair = Assert.Single(result.Pairs);
        Assert.True(result.ExcludedPersonId.HasValue);
        Assert.NotEqual(result.ExcludedPersonId.Value, pair.FirstId);
        Assert.NotEqual(result.ExcludedPersonId.Value, pair.SecondId);
        Assert.Contains(result.ExcludedPersonId.Value, participantIds);
    }
    
    [Fact]
    public void Detect_ParticipantIdsWithHistory_ShouldBeSelectNewPairs()
    {
        var item1 = _fixture.Create<long>();
        var item2 = _fixture.Create<long>();
        var item3 = _fixture.Create<long>();
        var item4 = _fixture.Create<long>();
        var historyPairs1 = new PersonPair(item1, item2);
        var historyPairs2 = new PersonPair(item3, item4);
        var history = new[] { historyPairs1, historyPairs2 };
        var orderedHistory = new[] { history };
        var participantIds = new[] { item1, item2, item3, item4 };
        
        var strategy = new SelectPairsStrategy(participantIds, orderedHistory);

        var result = strategy.Detect();
        
        Assert.False(IsIntersection(result.Pairs));
        Assert.False(IsIntersection(result.Pairs, history));
        Assert.Null(result.ExcludedPersonId);
    }

    private static bool IsIntersection(IReadOnlyCollection<PersonPair> pairs)
    {
        if (pairs is null)
            throw new ArgumentNullException(nameof(pairs));

        var exists = new HashSet<long>();

        foreach (var pair in pairs)
        {
            if (!exists.Add(pair.FirstId))
                return true;
            if (!exists.Add(pair.SecondId))
                return true;
        }

        return false;
    }

    private static bool IsIntersection(
        IReadOnlyCollection<PersonPair> pairs,
        IReadOnlyCollection<PersonPair> historyPairs)
    {
        if (pairs is null)
            throw new ArgumentNullException(nameof(pairs));
        if (historyPairs is null)
            throw new ArgumentNullException(nameof(historyPairs));
        
        foreach (var pair in pairs)
            if (historyPairs.Any(h => h.IsEquivalent(pair)))
                return true;

        return false;
    }
}