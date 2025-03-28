namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public sealed record ReviewTeamMetrics(
    TimeSpan FirstTouch,
    TimeSpan Review,
    TimeSpan Correction,
    int Iterations,
    int Total)
{
    public static ReviewTeamMetrics Empty { get; } = new(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, 0, 0);
    
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
    
    private int Add(int average, int value, int count)
    {
        return (average * Total + value * count) / (Total + count);
    }
}