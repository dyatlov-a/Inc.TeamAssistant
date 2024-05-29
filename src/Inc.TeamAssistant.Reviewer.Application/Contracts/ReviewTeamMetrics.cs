using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public sealed record ReviewTeamMetrics(
    TimeSpan FirstTouch,
    TimeSpan Review,
    TimeSpan Correction,
    decimal Iterations,
    int Total)
{
    public static ReviewTeamMetrics Empty { get; } = new(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, 0, 0);
    
    public static async Task<ReviewTeamMetrics> CreateFrom(
        TaskForReview taskForReview,
        Func<DateTimeOffset, DateTimeOffset, CancellationToken, Task<TimeSpan>> calculate,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(calculate);

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
                    firstTouch += await calculate(start, interval.End, token);
                    break;
                case TaskForReviewState.InProgress when interval.UserId == taskForReview.ReviewerId:
                    review += await calculate(start, interval.End, token);
                    break;
                case TaskForReviewState.OnCorrection when interval.UserId == taskForReview.OwnerId:
                    correction += await calculate(start, interval.End, token);
                    iterations++;
                    break;
            }

            start = interval.End;
        }
        
        return new ReviewTeamMetrics(firstTouch, review, correction, iterations, Total: 1);
    }
    
    public ReviewTeamMetrics Add(ReviewTeamMetrics reviewTeamMetrics)
    {
        ArgumentNullException.ThrowIfNull(reviewTeamMetrics);
        
        return new ReviewTeamMetrics(
            Add(FirstTouch, reviewTeamMetrics.FirstTouch, reviewTeamMetrics.Total),
            Add(Review, reviewTeamMetrics.Review, reviewTeamMetrics.Total),
            Add(Correction, reviewTeamMetrics.Correction, reviewTeamMetrics.Total),
            Add(Iterations, reviewTeamMetrics.Iterations, reviewTeamMetrics.Total),
            Total + reviewTeamMetrics.Total);
    }

    private TimeSpan Add(TimeSpan average, TimeSpan value, int count)
    {
        return (average * Total + value * count) / (Total + count);
    }

    private decimal Add(decimal average, decimal value, int count)
    {
        return (average * Total + value * count) / (Total + count);
    }
}