using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetAverageByTeam;

internal sealed class GetAverageByTeamQueryHandler : IRequestHandler<GetAverageByTeamQuery, GetAverageByTeamResult>
{
    private readonly ITaskForReviewReader _reader;
    private readonly ReviewTeamMetricsFactory _metricsFactory;

    public GetAverageByTeamQueryHandler(ITaskForReviewReader reader, ReviewTeamMetricsFactory metricsFactory)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
    }

    public async Task<GetAverageByTeamResult> Handle(GetAverageByTeamQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var tasks = await _reader.GetTasksFrom(query.TeamId, query.From.ToDateTimeOffset(), token);
        var tasksByDays = tasks.ToLookup(t => t.Created.ToDateOnly());
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