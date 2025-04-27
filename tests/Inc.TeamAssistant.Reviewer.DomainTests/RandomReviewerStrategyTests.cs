using AutoFixture;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.DomainTests;

public sealed class RandomReviewerStrategyTests
{
    private readonly IReadOnlyCollection<long> _teammates;
    private readonly Fixture _fixture = new();

    public RandomReviewerStrategyTests()
    {
        _teammates =
        [
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>(),
            _fixture.Create<long>()
        ];
    }

    [Fact]
    public void Constructor_TeamIsNull_ThrowsException()
    {
        RandomReviewerStrategy Action() => new(
            teammatesPool: null!,
            new Dictionary<long, int>());

        Assert.Throws<ArgumentNullException>(Action);
    }
    
    [Fact]
    public void Constructor_HistoryIsNull_ThrowsException()
    {
        RandomReviewerStrategy Action() => new(
            new TeammatesPool(_teammates, _fixture.Create<long>(), ReviewerId: null, LastReviewerId: null),
            null!);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_Team_ShouldNotOwner()
    {
        var ownerId = _teammates.First();
        var target = new RandomReviewerStrategy(
            new TeammatesPool(_teammates, ownerId, ReviewerId: null, LastReviewerId: null),
            new Dictionary<long, int>());

        var reviewerId = target.GetReviewer();
        
        Assert.NotEqual(ownerId, reviewerId);
    }
    
    [Fact]
    public void Next_Team_ShouldNotLastReviewerId()
    {
        var ownerId = _teammates.First();
        var lastReviewerId = _teammates.Skip(1).First();
        var target = new RandomReviewerStrategy(
            new TeammatesPool(_teammates, ownerId, ReviewerId: null, lastReviewerId),
            new Dictionary<long, int>());

        var reviewerId = target.GetReviewer();
        
        Assert.NotEqual(lastReviewerId, reviewerId);
    }

    [Theory]
    [InlineData(1_000, 200)]
    public void Next_MultipleIterations_MustRandomReviewer(int iterationCount, int reviewerCountByPlayer)
    {
        var ownerId = _teammates.First();
        var target = new RandomReviewerStrategy(
            new TeammatesPool(_teammates, ownerId, ReviewerId: null, LastReviewerId: null),
            new Dictionary<long, int>());
        var reviewerIds = Enumerable.Range(0, iterationCount)
            .Select(_ => target.GetReviewer())
            .GroupBy(i => i)
            .ToDictionary(i => i, i => i.Count());
        
        Assert.Equal(_teammates.Count - 1, reviewerIds.Keys.Count);
        Assert.Equal(iterationCount, reviewerIds.Sum(r => r.Value));
        foreach (var reviewerId in reviewerIds)
            Assert.True(reviewerId.Value > reviewerCountByPlayer);
    }
}