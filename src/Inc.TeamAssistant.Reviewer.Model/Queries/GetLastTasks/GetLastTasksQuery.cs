using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

public sealed record GetLastTasksQuery(Guid TeamId, int Count)
    : IRequest<GetLastTasksResult>;