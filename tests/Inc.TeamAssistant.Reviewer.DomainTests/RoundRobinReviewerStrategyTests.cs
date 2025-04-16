using AutoFixture;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.DomainTests;

public sealed class RoundRobinReviewerStrategyTests
{
    private readonly IReadOnlyCollection<long> _teammates;
    private readonly Fixture _fixture = new();

    public RoundRobinReviewerStrategyTests()
    {
        _teammates = new[]
        {
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        };
    }
    
    [Fact]
    public void Constructor_TeamIsNull_ThrowsException()
    {
        RoundRobinReviewerStrategy Action() => new(
            teammates: null!,
            excludedPersonIds: Array.Empty<long>(),
            lastReviewerId: null);

        Assert.Throws<ArgumentNullException>(Action);
    }
    
    [Fact]
    public void Constructor_HistoryIsNull_ThrowsException()
    {
        RoundRobinReviewerStrategy Action() => new(
            _teammates,
            null!,
            lastReviewerId: null);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_Team_ShouldNotOwner()
    {
        var ownerId = _teammates.First();
        var target = new RoundRobinReviewerStrategy(_teammates, [ownerId], lastReviewerId: null);
        
        var reviewerId = target.GetReviewer();
        
        Assert.NotEqual(ownerId, reviewerId);
    }
    
    [Fact]
    public void Next_Team_ShouldNotLastReviewerId()
    {
        var ownerId = _teammates.First();
        var lastReviewerId = _teammates.Skip(1).First();
        var target = new RoundRobinReviewerStrategy(_teammates, [ownerId], lastReviewerId);

        var reviewerId = target.GetReviewer();
        
        Assert.NotEqual(lastReviewerId, reviewerId);
    }

    [Fact]
    public void Next_MultipleIterations_ShouldBeRoundRobin()
    {
        var ownerId = _teammates.First();
        var otherPlayerIds = _teammates
            .Where(p => p != ownerId)
            .OrderBy(p => p)
            .ToArray();

        long? lastReviewerId = null;
        foreach (var otherPlayerId in otherPlayerIds.Concat(otherPlayerIds))
        {
            var target = new RoundRobinReviewerStrategy(_teammates, [ownerId], lastReviewerId);
            var reviewerId = target.GetReviewer();
            lastReviewerId = reviewerId;
            Assert.Equal(otherPlayerId, reviewerId);
        }
    }
}