using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks.Converters;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks;

internal sealed class GetLastTasksQueryHandler : IRequestHandler<GetLastTasksQuery, GetLastTasksResult>
{
    private readonly ITaskForReviewReader _reader;
    private readonly ReviewTeamMetricsFactory _metricsFactory;

    public GetLastTasksQueryHandler(ITaskForReviewReader reader, ReviewTeamMetricsFactory metricsFactory)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
    }

    public async Task<GetLastTasksResult> Handle(GetLastTasksQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var tasks = await _reader.GetLastTasks(query.TeamId, query.From.ToDateTimeOffset(), token);
        var results = new List<TaskForReviewDto>();

        foreach (var task in tasks)
        {
            var stats = await _metricsFactory.Create(task, token);
            results.Add(TaskForReviewHistoryConverter.ConvertTo(task, stats));
        }

        return new GetLastTasksResult(results);
    }
}