using AutoFixture;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.DomainTests.Extensions;
using Xunit;

namespace Inc.TeamAssistant.RandomCoffee.DomainTests;

public sealed class SelectPairsStrategyTests
{
    private readonly Fixture _fixture = new();
    
    [Fact]
    public void Detect_Participants_SelectedDifferentPairs()
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
        
        Assert.False(result.Pairs.IsIntersection());
        Assert.Null(result.ExcludedPersonId);
    }
    
    [Fact]
    public void Detect_Participants_SelectedPairsWithExcludedParticipant()
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
    
    [Theory]
    [InlineData(1_000, 600)]
    public void Detect_MultipleTimes_SelectedAllParticipantIds(int iterationCount, int minSelectLimit)
    {
        var orderedHistory = Array.Empty<PersonPair[]>();
        var participantIds = new[]
        {
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        };
        
        var strategy = new SelectPairsStrategy(participantIds, orderedHistory);

        var excludedNotEqualsCount = 0;
        long? previousExcludedPersonId = null;
        for (var index = 0; index < iterationCount; index++)
        {
            var detectResult = strategy.Detect(lastExcludedPersonId: null);
            var pair = Assert.Single(detectResult.Pairs);
            
            Assert.True(detectResult.ExcludedPersonId.HasValue);
            Assert.NotEqual(detectResult.ExcludedPersonId.Value, pair.FirstId);
            Assert.NotEqual(detectResult.ExcludedPersonId.Value, pair.SecondId);
            Assert.Contains(detectResult.ExcludedPersonId.Value, participantIds);
            
            if (previousExcludedPersonId != detectResult.ExcludedPersonId)
                excludedNotEqualsCount++;
            
            previousExcludedPersonId = detectResult.ExcludedPersonId;
        }
        
        Assert.True(excludedNotEqualsCount > minSelectLimit);
    }
    
    [Theory]
    [InlineData(1_000, 500)]
    public void Detect_ParticipantsAndHistory_SelectedPairs(int iterationCount, int minSelectLimit)
    {
        var item1 = _fixture.Create<long>();
        var item2 = _fixture.Create<long>();
        var item3 = _fixture.Create<long>();
        var item4 = _fixture.Create<long>();
        var participantIds = new[] { item1, item2, item3, item4 };
        var history = new List<PersonPair[]>();
        var notIntersectionCount = 0;

        for (var index = 0; index < iterationCount; index++)
        {
            var currentHistory = history.GetHistoryPart();
            var strategy = new SelectPairsStrategy(participantIds, currentHistory);

            var result = strategy.Detect(lastExcludedPersonId: null);
        
            Assert.False(result.Pairs.IsIntersection());
            Assert.Null(result.ExcludedPersonId);

            if (history.Any() && !result.Pairs.IsIntersection(history.Last()))
                notIntersectionCount++;
                    
            history.Add(result.Pairs.ToArray());
        }
        
        Assert.True(notIntersectionCount > minSelectLimit);
    }
    
    [Theory]
    [InlineData(1_000)]
    public void Detect_ParticipantsAndHistory_SelectedPairsWithoutDuplicates(int iterationCount)
    {
        var item1 = _fixture.Create<long>();
        var item2 = _fixture.Create<long>();
        var item3 = _fixture.Create<long>();
        var item4 = _fixture.Create<long>();
        var participantIds = new[] { item1, item2, item3, item4 };
        var history = new List<PersonPair[]>();

        for (var index = 0; index < iterationCount; index++)
        {
            var currentHistory = history.GetHistoryPart();
            var strategy = new SelectPairsStrategy(participantIds, currentHistory);

            var result = strategy.Detect(lastExcludedPersonId: null);
        
            Assert.False(result.Pairs.IsIntersection());
            Assert.All(result.Pairs, p => Assert.True(p.FirstId != p.SecondId));
                    
            history.Add(result.Pairs.ToArray());
        }
    }
    
    [Theory]
    [InlineData(1_000, 900)]
    public void Detect_NewParticipantsAndHistory_SelectedNewPairs(int iterationCount, int minSelectLimit)
    {
        var item1 = _fixture.Create<long>();
        var item2 = _fixture.Create<long>();
        var item3 = _fixture.Create<long>();
        var item4 = _fixture.Create<long>();
        var participantIds = new[] { item1, item2, item3, item4 };
        var history = new List<PersonPair[]>();
        var includeNewParticipantCount = 0;

        for (var index = 0; index < iterationCount; index++)
        {
            var newParticipant = _fixture.Create<long>();
            participantIds[0] = newParticipant;
            var currentHistory = history.GetHistoryPart();
            var strategy = new SelectPairsStrategy(participantIds, currentHistory);

            var result = strategy.Detect(lastExcludedPersonId: null);
        
            Assert.False(result.Pairs.IsIntersection());
            Assert.Equal(2, result.Pairs.Count);
            Assert.Null(result.ExcludedPersonId);

            if (result.Pairs.Any(p => p.FirstId == newParticipant || p.SecondId == newParticipant))
                includeNewParticipantCount++;
                    
            history.Add(result.Pairs.ToArray());
        }
        
        Assert.True(includeNewParticipantCount > minSelectLimit);
    }
    
    [Theory]
    [InlineData(100, 90)]
    public void Detect_ALotOfNewParticipants_SelectedPairs(int iterationCount, int minSelectLimit)
    {
        var participantIds = _fixture.CreateMany<long>(21).ToArray();
        var history = new List<PersonPair[]>();
        var includeNewParticipantCount = 0;

        for (var index = 0; index < iterationCount; index++)
        {
            var newParticipant = _fixture.Create<long>();
            participantIds[0] = newParticipant;
            var currentHistory = history.GetHistoryPart();
            var strategy = new SelectPairsStrategy(participantIds, currentHistory);

            var result = strategy.Detect(lastExcludedPersonId: null);
        
            Assert.False(result.Pairs.IsIntersection());
            Assert.Equal(participantIds.Length / 2, result.Pairs.Count);
            Assert.Equal(participantIds.Length % 2 == 0, !result.ExcludedPersonId.HasValue);
                    
            if (result.Pairs.Any(p => p.FirstId == newParticipant || p.SecondId == newParticipant))
                includeNewParticipantCount++;
            
            history.Add(result.Pairs.ToArray());
        }
        
        Assert.True(includeNewParticipantCount > minSelectLimit);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Detect_TwoParticipantsAndHistory_SelectedPairs(int historyCount)
    {
        var item1 = _fixture.Create<long>();
        var item2 = _fixture.Create<long>();
        var history = new List<PersonPair>();

        for (var index = 0; index < historyCount; index++)
            history.Add(new PersonPair(item1, item2));
        
        var orderedHistory = new[] { history.ToArray() };
        var participantIds = new[] { item1, item2 };
        
        var strategy = new SelectPairsStrategy(participantIds, orderedHistory);

        var result = strategy.Detect(lastExcludedPersonId: null);
        
        Assert.True(result.Pairs.IsIntersection(history));
        Assert.Null(result.ExcludedPersonId);
    }
}