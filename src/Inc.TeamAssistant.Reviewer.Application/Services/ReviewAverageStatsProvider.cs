using System.Collections.Concurrent;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewAverageStatsProvider : IReviewAverageStatsProvider
{
    private readonly ConcurrentDictionary<Guid, ReviewAverageStats> _statsByTeams = new();
    private bool _isInitialized;
    
    public void Initialize(IReadOnlyCollection<TaskForReview> taskForReviews)
    {
        ArgumentNullException.ThrowIfNull(taskForReviews);

        foreach (var taskForReview in taskForReviews.Where(t => t.ReviewIntervals.Any()))
            Add(taskForReview);

        _isInitialized = true;
    }

    void IReviewAverageStatsProvider.Add(TaskForReview taskForReview)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        if (_isInitialized)
            Add(taskForReview);
    }

    public ReviewAverageStats Get(Guid teamId) => _statsByTeams.GetValueOrDefault(teamId, ReviewAverageStats.Empty);
    
    private void Add(TaskForReview taskForReview)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var reviewAverageStats = ReviewAverageStats.CreateFrom(taskForReview.ReviewIntervals);

        _statsByTeams.AddOrUpdate(taskForReview.TeamId, reviewAverageStats, (_, v) => v.Add(reviewAverageStats));
    }
}