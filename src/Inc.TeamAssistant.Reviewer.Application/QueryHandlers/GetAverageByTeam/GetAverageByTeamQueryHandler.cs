using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetAverageByTeam;

internal sealed class GetAverageByTeamQueryHandler : IRequestHandler<GetAverageByTeamQuery, GetAverageByTeamResult>
{
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly ReviewTeamMetricsFactory _metricsFactory;

    public GetAverageByTeamQueryHandler(
        ITaskForReviewReader taskForReviewReader,
        ReviewTeamMetricsFactory metricsFactory)
    {
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
    }

    public async Task<GetAverageByTeamResult> Handle(GetAverageByTeamQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var from = DateTimeOffset.UtcNow.AddDays(-query.Depth);
        var tasks = await _taskForReviewReader.GetTasksFrom(query.TeamId, from, token);
        var tasksByDays = tasks.ToLookup(t => new DateOnly(t.Created.Year, t.Created.Month, t.Created.Day));
        ReviewAverageStatsDto? previous = null;
        var items = new List<ReviewAverageStatsDto>();

        foreach (var tasksByDay in tasksByDays.OrderBy(t => t.Key))
        {
            var current = ReviewTeamMetrics.Empty;
            
            foreach (var task in tasksByDay)
            {
                var metrics = await _metricsFactory.Create(task, token);
                current = current == ReviewTeamMetrics.Empty
                    ? metrics
                    : current.Add(metrics);
            }

            if (current.FirstTouch != TimeSpan.Zero ||
                current.Review != TimeSpan.Zero ||
                current.Correction != TimeSpan.Zero)
            {
                previous = new ReviewAverageStatsDto(
                    tasksByDay.Key,
                    GetValue(current.FirstTouch, previous?.FirstTouch),
                    GetValue(current.Review, previous?.Review),
                    GetValue(current.Correction, previous?.Correction));
                
                items.Add(previous);
            }
        }

        return new GetAverageByTeamResult(items);
    }

    private TimeSpan GetValue(TimeSpan current, TimeSpan? previous)
    {
        if (current != TimeSpan.Zero)
            return current;
        
        return previous ?? TimeSpan.Zero;
    }
}