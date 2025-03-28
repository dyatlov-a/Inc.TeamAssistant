using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks.Converters;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks;

internal sealed class GetLastTasksQueryHandler : IRequestHandler<GetLastTasksQuery, GetLastTasksResult>
{
    private readonly ITaskForReviewReader _reader;
    private readonly TaskForReviewHistoryConverter _converter;

    public GetLastTasksQueryHandler(ITaskForReviewReader reader, TaskForReviewHistoryConverter converter)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public async Task<GetLastTasksResult> Handle(GetLastTasksQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var tasks = await _reader.GetLastTasks(query.TeamId, query.From.ToDateTimeOffset(), token);
        var results = new List<TaskForReviewDto>();

        foreach (var task in tasks)
            results.Add(await _converter.ConvertTo(task, token));

        return new GetLastTasksResult(results);
    }
}