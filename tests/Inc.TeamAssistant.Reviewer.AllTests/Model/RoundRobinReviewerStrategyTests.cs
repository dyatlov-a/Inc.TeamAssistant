using AutoFixture;
using Inc.TeamAssistant.Reviewer.All.Model;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.AllTests.Model;

public sealed class RoundRobinReviewerStrategyTests
{
    private readonly Team _team;
    private readonly INextReviewerStrategy _target;
    private readonly Fixture _fixture = new();

    public RoundRobinReviewerStrategyTests()
    {
        _team = new(_fixture.Create<long>(), _fixture.Create<string>(), NextReviewerType.RoundRobin);
        _team.AddPlayer(_fixture.Create<Person>());
        _team.AddPlayer(_fixture.Create<Person>());
        _team.AddPlayer(_fixture.Create<Person>());
        _team.AddPlayer(_fixture.Create<Person>());
        _target = _team.NextReviewerStrategy;
    }
    
    [Fact]
    public void Constructor_TeamIsNull_ThrowsException()
    {
        RandomReviewerStrategy Action() => new(team: null!, _fixture.Create<int>());

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_OwnerIsNull_ThrowsException()
    {
        Player Action() => _target.Next(owner: null!);

        Assert.Throws<ArgumentNullException>(Action);
    }

    [Fact]
    public void Next_Team_ShouldNotOwner()
    {
        var owner = _team.Players.First();

        var reviewer = _target.Next(owner);
        
        Assert.NotEqual(owner.Id, reviewer.Id);
    }
    
    [Fact]
    public void Next_Team_ShouldNotLastReviewerId()
    {
        var owner = _team.Players.First();
        var lastReviewer = _target.Next(owner);
        typeof(Player).GetProperty(nameof(Player.LastReviewerId))!.SetValue(owner, lastReviewer.Person.Id);
        
        var reviewer = _target.Next(owner);
        
        Assert.NotEqual(lastReviewer.Id, reviewer.Id);
    }

    [Fact]
    public void Next_MultipleIterations_ShouldBeRoundRobin()
    {
        var owner = _team.Players.First();
        var otherPlayers = _team.Players
            .Where(p => p.Person.Id != owner.Person.Id)
            .OrderBy(p => p.Person.Id)
            .ToArray();

        foreach (var otherPlayer in otherPlayers.Concat(otherPlayers))
        {
            var reviewer = _target.Next(owner);
            typeof(Player).GetProperty(nameof(Player.LastReviewerId))!.SetValue(owner, reviewer.Person.Id);
            Assert.Equal(otherPlayer.Person.Id, reviewer.Person.Id);
        }
    }
}