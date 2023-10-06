using AutoFixture;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.DomainTests;

public sealed class RandomReviewerStrategyTests
{
    private readonly Team _team;
    private readonly INextReviewerStrategy _target;
    private readonly Fixture _fixture = new();

    public RandomReviewerStrategyTests()
    {
        _team = new(_fixture.Create<long>(), _fixture.Create<string>(), NextReviewerType.Random);
        _team.AddPlayer(_fixture.Create<Person>());
        _team.AddPlayer(_fixture.Create<Person>());
        _team.AddPlayer(_fixture.Create<Person>());
        _team.AddPlayer(_fixture.Create<Person>());
        _target = _team.NextReviewerStrategy;
    }

    [Fact]
    public void Constructor_TeamIsNull_ThrowsException()
    {
        RandomReviewerStrategy Action() => new(team: null!);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_OwnerIsNull_ThrowsException()
    {
        Person Action() => _target.Next(owner: null!, lastReviewer: null);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_Team_ShouldNotOwner()
    {
        var owner = _team.Players.First();

        var reviewer = _target.Next(owner, lastReviewer: null);
        
        Assert.NotEqual(owner.Id, reviewer.Id);
    }
    
    [Fact]
    public void Next_Team_ShouldNotLastReviewerId()
    {
        var owner = _team.Players.First();
        var lastReviewer = _team.Players.Skip(1).First();

        var reviewer = _target.Next(owner, lastReviewer);
        
        Assert.NotEqual(lastReviewer.Id, reviewer.Id);
    }

    [Theory]
    [InlineData(1_000, 300)]
    public void Next_MultipleIterations_MustRandomReviewer(int iterationCount, int reviewerCountByPlayer)
    {
        var owner = _team.Players.First();

        var reviewers = Enumerable.Range(0, iterationCount)
            .Select(_ => _target.Next(owner, lastReviewer: null).Id)
            .GroupBy(i => i)
            .ToDictionary(i => i, i => i.Count());
        
        Assert.Equal(_team.Players.Count - 1, reviewers.Keys.Count);
        foreach (var reviewer in reviewers)
            Assert.True(reviewer.Value > reviewerCountByPlayer);
    }
}