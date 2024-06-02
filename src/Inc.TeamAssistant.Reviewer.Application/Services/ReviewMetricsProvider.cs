using System.Collections.Concurrent;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMetricsProvider : IReviewMetricsProvider, IReviewMetricsLoader
{
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

    async Task IReviewMetricsProvider.Add(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        await Add(taskForReview, token);
    }

    public ReviewTeamMetrics Get(Guid teamId) => _statsByTeams.GetValueOrDefault(teamId, ReviewTeamMetrics.Empty);
    
    private async Task Add(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var reviewAverageStats = await ReviewTeamMetrics.CreateFrom(
            taskForReview,
            _holidayService.CalculateWorkTime,
            token);

        _statsByTeams.AddOrUpdate(taskForReview.TeamId, reviewAverageStats, (_, v) => v.Add(reviewAverageStats));
    }
}