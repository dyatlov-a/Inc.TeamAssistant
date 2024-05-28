using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public sealed record ReviewAverageStats(
    TimeSpan FirstTouch,
    TimeSpan Review,
    TimeSpan Correction,
    decimal Iterations,
    int Total)
{
    public static ReviewAverageStats Empty { get; } = new(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, 0, 0);
    
    public static ReviewAverageStats CreateFrom(IReadOnlyCollection<ReviewInterval> reviewIntervals)
    {
        ArgumentNullException.ThrowIfNull(reviewIntervals);

        var iterations = reviewIntervals.Count(i => i.State == TaskForReviewState.OnCorrection);

        return new ReviewAverageStats(
            SumBy(reviewIntervals, TaskForReviewState.New),
            SumBy(reviewIntervals, TaskForReviewState.InProgress),
            SumBy(reviewIntervals, TaskForReviewState.OnCorrection),
            iterations,
            Total: 1);
    }
    
    public ReviewAverageStats Add(ReviewAverageStats reviewAverageStats)
    {
        ArgumentNullException.ThrowIfNull(reviewAverageStats);
        
        return new ReviewAverageStats(
            Add(FirstTouch, reviewAverageStats.FirstTouch, reviewAverageStats.Total),
            Add(Review, reviewAverageStats.Review, reviewAverageStats.Total),
            Add(Correction, reviewAverageStats.Correction, reviewAverageStats.Total),
            Add(Iterations, reviewAverageStats.Iterations, reviewAverageStats.Total),
            Total + reviewAverageStats.Total);
    }

    private TimeSpan Add(TimeSpan average, TimeSpan value, int count) => (average * Total + value * count) / (Total + count);
    
    private decimal Add(decimal average, decimal value, int count) => (average * Total + value * count) / (Total + count);

    private static TimeSpan SumBy(IReadOnlyCollection<ReviewInterval> reviewIntervals, TaskForReviewState state)
    {
        ArgumentNullException.ThrowIfNull(reviewIntervals);

        var total = reviewIntervals
            .Where(i => i.State == state)
            .Select(i => i.End - i.Begin)
            .Sum(i => i.Ticks);
        
        return new TimeSpan(total);
    }
}