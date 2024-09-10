using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks.Converters;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks;

internal sealed class GetLastTasksQueryHandler : IRequestHandler<GetLastTasksQuery, GetLastTasksResult>
{
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly TaskForReviewHistoryConverter _converter;

    public GetLastTasksQueryHandler(
        ITaskForReviewReader taskForReviewReader,
        TaskForReviewHistoryConverter converter)
    {
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public async Task<GetLastTasksResult> Handle(GetLastTasksQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var from = new DateTimeOffset(query.From, TimeOnly.MinValue, TimeSpan.Zero);
        var tasks = await _taskForReviewReader.GetLastTasks(query.TeamId, from, token);
        var results = new List<TaskForReviewDto>();

        foreach (var task in tasks)
            results.Add(await _converter.ConvertTo(task, token));

        return new GetLastTasksResult(results);
    }
}