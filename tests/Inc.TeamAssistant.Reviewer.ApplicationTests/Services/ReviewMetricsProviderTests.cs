using AutoFixture;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Holidays.Internal;
using Inc.TeamAssistant.Holidays.Model;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.ApplicationTests.Services;

public sealed class ReviewMetricsProviderTests
{
    private readonly Fixture _fixture = new();
    private readonly IReviewMetricsProvider _target;

    public ReviewMetricsProviderTests()
    {
        var calendar = new Calendar(
            _fixture.Create<Guid>(),
            _fixture.Create<long>(),
            schedule: null);
        
        var holidayReader = Substitute.For<IHolidayReader>();
        holidayReader.Find(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(calendar);
        var holidayService = new HolidayService(holidayReader);
        _target = new ReviewMetricsProvider(new ReviewTeamMetricsFactory(holidayService));
    }

    [Theory]
    [InlineData("01:00:00", "00:10:00", "00:30:00", "00:05:00")]
    [InlineData("02:00:00", "00:30:00", "01:00:00", "00:10:00")]
    public async Task Add_IntervalsByTask_CorrectMetrics(
        string moveToInProgressDuration,
        string declineDuration,
        string moveToNextRoundDuration,
        string acceptDuration)
    {
        var moveToInProgressInterval = TimeSpan.Parse(moveToInProgressDuration);
        var declineDurationInterval = TimeSpan.Parse(declineDuration);
        var moveToNextRoundDurationInterval = TimeSpan.Parse(moveToNextRoundDuration);
        var acceptDurationInterval = TimeSpan.Parse(acceptDuration);
        var teamId = _fixture.Create<Guid>();
        var start = DateTimeOffset.UtcNow;

        var taskForReview = CreateAndSetupTaskForReview(
            teamId,
            start,
            moveToInProgressInterval,
            acceptDurationInterval,
            new[] { (declineDurationInterval, moveToNextRoundDurationInterval) });
        await _target.Add(taskForReview, CancellationToken.None);
        
        var reviewTeamMetrics = _target.Get(teamId);
        
        Assert.Equal(moveToInProgressInterval, reviewTeamMetrics.FirstTouch);
        Assert.Equal(moveToNextRoundDurationInterval, reviewTeamMetrics.Correction);
        Assert.Equal(declineDurationInterval + acceptDurationInterval, reviewTeamMetrics.Review);
        Assert.Equal(1, reviewTeamMetrics.Iterations);
        Assert.Equal(1, reviewTeamMetrics.Total);
    }
    
    [Theory]
    [InlineData("01:00:00", "00:10:00", "00:30:00", "00:05:00")]
    [InlineData("02:00:00", "00:30:00", "01:00:00", "00:10:00")]
    public async Task Add_IntervalsByTaskWithManyRounds_CorrectMetrics(
        string moveToInProgressDuration,
        string declineDuration,
        string moveToNextRoundDuration,
        string acceptDuration)
    {
        var moveToInProgressInterval = TimeSpan.Parse(moveToInProgressDuration);
        var declineDurationInterval = TimeSpan.Parse(declineDuration);
        var moveToNextRoundDurationInterval = TimeSpan.Parse(moveToNextRoundDuration);
        var acceptDurationInterval = TimeSpan.Parse(acceptDuration);
        var teamId = _fixture.Create<Guid>();
        var start = DateTimeOffset.UtcNow;

        var taskForReview = CreateAndSetupTaskForReview(
            teamId,
            start,
            moveToInProgressInterval,
            acceptDurationInterval,
            new[]
            {
                (declineDurationInterval * 2, moveToNextRoundDurationInterval * 2),
                (declineDurationInterval, moveToNextRoundDurationInterval)
            });
        await _target.Add(taskForReview, CancellationToken.None);
        
        var reviewTeamMetrics = _target.Get(teamId);
        
        Assert.Equal(moveToInProgressInterval, reviewTeamMetrics.FirstTouch);
        Assert.Equal(moveToNextRoundDurationInterval * 3, reviewTeamMetrics.Correction);
        Assert.Equal(declineDurationInterval * 3 + acceptDurationInterval, reviewTeamMetrics.Review);
        Assert.Equal(2, reviewTeamMetrics.Iterations);
        Assert.Equal(1, reviewTeamMetrics.Total);
    }
    
    [Theory]
    [InlineData("01:00:00", "00:10:00", "00:30:00", "00:05:00")]
    [InlineData("02:00:00", "00:30:00", "01:00:00", "00:10:00")]
    public async Task Add_IntervalsByTasks_CorrectAverageMetrics(
        string moveToInProgressDuration,
        string declineDuration,
        string moveToNextRoundDuration,
        string acceptDuration)
    {
        var moveToInProgressInterval = TimeSpan.Parse(moveToInProgressDuration);
        var declineDurationInterval = TimeSpan.Parse(declineDuration);
        var moveToNextRoundDurationInterval = TimeSpan.Parse(moveToNextRoundDuration);
        var acceptDurationInterval = TimeSpan.Parse(acceptDuration);
        var teamId = _fixture.Create<Guid>();
        var start = DateTimeOffset.UtcNow;

        var taskForReviewFirst = CreateAndSetupTaskForReview(
            teamId,
            start,
            moveToInProgressInterval,
            acceptDurationInterval,
            new[] { (declineDurationInterval, moveToNextRoundDurationInterval) });
        var taskForReviewSecond = CreateAndSetupTaskForReview(
            teamId,
            start,
            moveToInProgressInterval * 2,
            acceptDurationInterval * 2,
            new[] { (declineDurationInterval * 2, moveToNextRoundDurationInterval * 2) });
        var taskForReviewFromOtherTeam = CreateAndSetupTaskForReview(
            _fixture.Create<Guid>(),
            start,
            moveToInProgressInterval,
            acceptDurationInterval,
            new[] { (declineDurationInterval, moveToNextRoundDurationInterval) });
        await _target.Add(taskForReviewFirst, CancellationToken.None);
        await _target.Add(taskForReviewSecond, CancellationToken.None);
        await _target.Add(taskForReviewFromOtherTeam, CancellationToken.None);
        
        var reviewTeamMetrics = _target.Get(teamId);
        
        Assert.Equal(moveToInProgressInterval * 3/2, reviewTeamMetrics.FirstTouch);
        Assert.Equal(moveToNextRoundDurationInterval * 3/2, reviewTeamMetrics.Correction);
        Assert.Equal(declineDurationInterval * 3/2 + acceptDurationInterval * 3/2, reviewTeamMetrics.Review);
        Assert.Equal(1, reviewTeamMetrics.Iterations);
        Assert.Equal(2, reviewTeamMetrics.Total);
    }

    private TaskForReview CreateAndSetupTaskForReview(
        Guid teamId,
        DateTimeOffset start,
        TimeSpan? moveToInProgressDuration = null,
        TimeSpan? acceptDuration = null,
        IReadOnlyCollection<(TimeSpan DeclineDuration, TimeSpan NextRoundDuration)>? reviewDurations = null)
    {
        var draft = new DraftTaskForReview(
            _fixture.Create<Guid>(),
            teamId,
            _fixture.Create<long>(),
            _fixture.Create<NextReviewerType>(),
            _fixture.Create<long>(),
            _fixture.Create<int>(),
            _fixture.Create<string>(),
            start);
        var taskForReview = new TaskForReview(
            _fixture.Create<Guid>(),
            draft,
            _fixture.Create<Guid>(),
            start,
            _fixture.Create<TimeSpan>(),
            _fixture.Create<long>());
        var operationStart = start;

        if (moveToInProgressDuration.HasValue)
        {
            operationStart = operationStart.Add(moveToInProgressDuration.Value);
            taskForReview.MoveToInProgress(_fixture.Create<TimeSpan>(), operationStart);
        }

        if (reviewDurations is not null)
            foreach (var reviewDuration in reviewDurations)
            {
                operationStart = operationStart.Add(reviewDuration.DeclineDuration);
                taskForReview.Decline(operationStart);

                operationStart = operationStart.Add(reviewDuration.NextRoundDuration);
                taskForReview.MoveToNextRound(operationStart);
            }

        if (acceptDuration.HasValue)
            taskForReview.Accept(operationStart.Add(acceptDuration.Value));

        return taskForReview;
    }
}