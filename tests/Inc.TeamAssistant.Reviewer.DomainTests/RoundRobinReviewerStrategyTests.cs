using AutoFixture;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.DomainTests;

public sealed class RoundRobinReviewerStrategyTests
{
    private readonly IReadOnlyCollection<long> _teammates;
    private readonly RoundRobinReviewerStrategy _target;
    private readonly Fixture _fixture = new();

    public RoundRobinReviewerStrategyTests()
    {
        _teammates = new[]
        {
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        };
        _target = new RoundRobinReviewerStrategy(_teammates);
    }
    
    [Fact]
    public void Constructor_TeamIsNull_ThrowsException()
    {
        RandomReviewerStrategy Action() => new(teammates: null!);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_Team_ShouldNotOwner()
    {
        var ownerId = _teammates.First();

        var reviewerId = _target.Next(ownerId, lastReviewerId: null);
        
        Assert.NotEqual(ownerId, reviewerId);
    }
    
    [Fact]
    public void Next_Team_ShouldNotLastReviewerId()
    {
        var ownerId = _teammates.First();
        var lastReviewerId = _teammates.Skip(1).First();
        
        var reviewerId = _target.Next(ownerId, lastReviewerId);
        
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
            var reviewerId = _target.Next(ownerId, lastReviewerId);
            lastReviewerId = reviewerId;
            Assert.Equal(otherPlayerId, reviewerId);
        }
    }
}