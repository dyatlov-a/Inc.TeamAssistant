using System.Collections.Concurrent;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMetricsProvider : IReviewMetricsProvider, IReviewMetricsLoader
{
    private readonly ConcurrentDictionary<Guid, ReviewTeamMetrics> _statsByTeams = new();
    private bool _isLoading = true;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(5);

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

        _isLoading = false;
    }

    async Task IReviewMetricsProvider.Add(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        if (!_isLoading)
            await Add(taskForReview, token);
    }

    public ReviewTeamMetrics Get(Guid teamId) => _statsByTeams.GetValueOrDefault(teamId, ReviewTeamMetrics.Empty);
    
    private async Task Add(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var reviewAverageStats = await ReviewTeamMetrics.CreateFrom(taskForReview, CalculateWorkInterval, token);

        _statsByTeams.AddOrUpdate(taskForReview.TeamId, reviewAverageStats, (_, v) => v.Add(reviewAverageStats));
    }
    
    private async Task<TimeSpan> CalculateWorkInterval(
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken token)
    {
        var interval = TimeSpan.Zero;
        var current = start + TimeWindow;

        while (current <= end)
        {
            if (await _holidayService.IsWorkTime(current, token))
                interval += TimeWindow;
            
            current += TimeWindow;
        }

        return interval;
    }
}