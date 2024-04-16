using AutoFixture;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.DomainTests;

public sealed class RandomReviewerStrategyTests
{
    private readonly IReadOnlyCollection<long> _teammates;
    private readonly RandomReviewerStrategy _target;
    private readonly Fixture _fixture = new();

    public RandomReviewerStrategyTests()
    {
        _teammates = new[]
        {
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        };
        _target = new RandomReviewerStrategy(_teammates, new Dictionary<long, int>());
    }

    [Fact]
    public void Constructor_TeamIsNull_ThrowsException()
    {
        RandomReviewerStrategy Action() => new(teammates: null!, new Dictionary<long, int>());

        Assert.Throws<ArgumentNullException>(Action);
    }
    
    [Fact]
    public void Constructor_HistoryIsNull_ThrowsException()
    {
        RandomReviewerStrategy Action() => new(_teammates, null!);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_Team_ShouldNotOwner()
    {
        var ownerId = _teammates.First();

        var reviewerId = _target.Next([ownerId], lastReviewerId: null);
        
        Assert.NotEqual(ownerId, reviewerId);
    }
    
    [Fact]
    public void Next_Team_ShouldNotLastReviewerId()
    {
        var ownerId = _teammates.First();
        var lastReviewerId = _teammates.Skip(1).First();

        var reviewerId = _target.Next([ownerId], lastReviewerId);
        
        Assert.NotEqual(lastReviewerId, reviewerId);
    }

    [Theory]
    [InlineData(1_000, 100)]
    public void Next_MultipleIterations_MustRandomReviewer(int iterationCount, int reviewerCountByPlayer)
    {
        var owner = _teammates.First();

        var reviewerIds = Enumerable.Range(0, iterationCount)
            .Select(_ => _target.Next([owner], lastReviewerId: null))
            .GroupBy(i => i)
            .ToDictionary(i => i, i => i.Count());
        
        Assert.Equal(_teammates.Count - 1, reviewerIds.Keys.Count);
        Assert.Equal(iterationCount, reviewerIds.Sum(r => r.Value));
        foreach (var reviewerId in reviewerIds)
            Assert.True(reviewerId.Value > reviewerCountByPlayer);
    }
}