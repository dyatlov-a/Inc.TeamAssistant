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
        var tasks = await _taskForReviewReader.GetTasksFrom(from, token);
        var tasksByDays = tasks.ToLookup(t => new DateOnly(t.Created.Year, t.Created.Month, t.Created.Day));
        
        var items = new List<ReviewAverageStatsDto>();

        foreach (var tasksByDay in tasksByDays.OrderBy(t => t.Key))
        {
            var averageByGroup = ReviewTeamMetrics.Empty;
            
            foreach (var task in tasksByDay)
            {
                var metrics = await _metricsFactory.Create(task, token);
                averageByGroup = averageByGroup == ReviewTeamMetrics.Empty
                    ? metrics
                    : averageByGroup.Add(metrics);
            }
            
            items.Add(new ReviewAverageStatsDto(
                tasksByDay.Key,
                averageByGroup.FirstTouch,
                averageByGroup.Review,
                averageByGroup.Correction));
        }

        return new GetAverageByTeamResult(items);
    }
}