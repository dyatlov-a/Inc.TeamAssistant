using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetLastTasks;

internal sealed class GetLastTasksQueryHandler : IRequestHandler<GetLastTasksQuery, GetLastTasksResult>
{
    private readonly ITaskForReviewReader _taskForReviewReader;

    public GetLastTasksQueryHandler(ITaskForReviewReader taskForReviewReader)
    {
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
    }

    public async Task<GetLastTasksResult> Handle(GetLastTasksQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var tasks = await _taskForReviewReader.GetLastTasks(query.TeamId, query.Count, token);

        return new GetLastTasksResult(tasks);
    }
}