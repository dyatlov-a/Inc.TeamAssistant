using System.Collections.Concurrent;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMetricsProvider : IReviewMetricsProvider, IReviewMetricsLoader
{
    private readonly ConcurrentDictionary<Guid, ReviewTeamMetrics> _statsByTeams = new();

    private readonly ReviewTeamMetricsFactory _metricsFactory;

    public ReviewMetricsProvider(ReviewTeamMetricsFactory metricsFactory)
    {
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
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

        var reviewAverageStats = await _metricsFactory.Create(taskForReview, token);

        _statsByTeams.AddOrUpdate(taskForReview.TeamId, reviewAverageStats, (_, v) => v.Add(reviewAverageStats));
    }
}