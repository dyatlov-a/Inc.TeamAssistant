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

        var result = strategy.Detect(lastExcludedPersonId: null);
        
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

        var result = strategy.Detect(lastExcludedPersonId: null);
        
        var pair = Assert.Single(result.Pairs);
        Assert.True(result.ExcludedPersonId.HasValue);
        Assert.NotEqual(result.ExcludedPersonId.Value, pair.FirstId);
        Assert.NotEqual(result.ExcludedPersonId.Value, pair.SecondId);
        Assert.Contains(result.ExcludedPersonId.Value, participantIds);
    }
    
    [Fact]
    public void Detect_MultipleTimes_ShouldBeSelectAllParticipantIds()
    {
        var orderedHistory = Array.Empty<PersonPair[]>();
        var participantIds = new[]
        {
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        };
        
        var strategy = new SelectPairsStrategy(participantIds, orderedHistory);

        var firstTime = strategy.Detect(lastExcludedPersonId: null);
        var secondTime = strategy.Detect(firstTime.ExcludedPersonId);
        
        var firstPair = Assert.Single(firstTime.Pairs);
        Assert.True(firstTime.ExcludedPersonId.HasValue);
        Assert.NotEqual(firstTime.ExcludedPersonId.Value, firstPair.FirstId);
        Assert.NotEqual(firstTime.ExcludedPersonId.Value, firstPair.SecondId);
        Assert.Contains(firstTime.ExcludedPersonId.Value, participantIds);
        
        var secondPair = Assert.Single(secondTime.Pairs);
        Assert.True(secondTime.ExcludedPersonId.HasValue);
        Assert.NotEqual(secondTime.ExcludedPersonId.Value, secondPair.FirstId);
        Assert.NotEqual(secondTime.ExcludedPersonId.Value, secondPair.SecondId);
        Assert.Contains(secondTime.ExcludedPersonId.Value, participantIds);
        
        Assert.NotEqual(firstTime.ExcludedPersonId, secondTime.ExcludedPersonId);
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

        var result = strategy.Detect(lastExcludedPersonId: null);
        
        Assert.False(IsIntersection(result.Pairs));
        Assert.False(IsIntersection(result.Pairs, history));
        Assert.Null(result.ExcludedPersonId);
    }
    
    [Fact]
    public void Detect_SecondMeeting_ShouldBeSelectPairs()
    {
        var item1 = _fixture.Create<long>();
        var item2 = _fixture.Create<long>();
        var historyPairs1 = new PersonPair(item1, item2);
        var history = new[] { historyPairs1 };
        var orderedHistory = new[] { history };
        var participantIds = new[] { item1, item2 };
        
        var strategy = new SelectPairsStrategy(participantIds, orderedHistory);

        var result = strategy.Detect(lastExcludedPersonId: null);
        
        Assert.True(IsIntersection(result.Pairs, history));
        Assert.Null(result.ExcludedPersonId);
    }

    private static bool IsIntersection(IReadOnlyCollection<PersonPair> pairs)
    {
        ArgumentNullException.ThrowIfNull(pairs);

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
        ArgumentNullException.ThrowIfNull(pairs);
        ArgumentNullException.ThrowIfNull(historyPairs);

        foreach (var pair in pairs)
            if (historyPairs.Any(h => h.Equals(pair)))
                return true;

        return false;
    }
}