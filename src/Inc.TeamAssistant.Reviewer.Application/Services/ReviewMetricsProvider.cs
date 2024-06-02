using System.Collections.Concurrent;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMetricsProvider : IReviewMetricsProvider, IReviewMetricsLoader
{
    private static readonly TimeSpan Correction = TimeSpan.Parse("0.00:00:00.0000001");
    private readonly ConcurrentDictionary<Guid, ReviewTeamMetrics> _statsByTeams = new();

    private readonly IHolidayService _holidayService;

    public ReviewMetricsProvider(IHolidayService holidayService)
    {
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
    }

    public async Task Load(IReadOnlyCollection<TaskForReview> taskForReviews, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReviews);

        foreach (var taskForReview in taskForReviews.Where(t => t.ReviewIntervals.Any()))
            await Add(taskForReview, token);
    }

    public async Task<ReviewTeamMetrics> Create(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var iterations = 0;
        var firstTouch = TimeSpan.Zero;
        var review = TimeSpan.Zero;
        var correction = TimeSpan.Zero;
        var start = taskForReview.Created;
        
        foreach (var interval in taskForReview.ReviewIntervals.OrderBy(i => i.End))
        {
            switch (interval.State)
            {
                case TaskForReviewState.New when interval.UserId == taskForReview.ReviewerId:
                    firstTouch += await _holidayService.CalculateWorkTime(start, interval.End, token);
                    break;
                case TaskForReviewState.InProgress when interval.UserId == taskForReview.ReviewerId:
                    review += await _holidayService.CalculateWorkTime(start, interval.End, token);
                    break;
                case TaskForReviewState.OnCorrection when interval.UserId == taskForReview.OwnerId:
                    correction += await _holidayService.CalculateWorkTime(start, interval.End, token);
                    iterations++;
                    break;
            }

            start = interval.End;
        }
        
        return new ReviewTeamMetrics(
            TryCorrection(firstTouch),
            TryCorrection(review),
            TryCorrection(correction),
            iterations,
            Total: 1);
    }

    private TimeSpan TryCorrection(TimeSpan value)
    {
        var valueWithCorrection = value.Add(Correction);
        return valueWithCorrection.Nanoseconds == 0 ? valueWithCorrection : value;
    }

    async Task IReviewMetricsProvider.Add(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        await Add(taskForReview, token);
    }

    public ReviewTeamMetrics Get(Guid teamId) => _statsByTeams.GetValueOrDefault(teamId, ReviewTeamMetrics.Empty);
    
    private async Task Add(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var reviewAverageStats = await Create(taskForReview, token);

        _statsByTeams.AddOrUpdate(taskForReview.TeamId, reviewAverageStats, (_, v) => v.Add(reviewAverageStats));
    }
}