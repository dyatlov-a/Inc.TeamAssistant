using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class ReviewService : IReviewService
{
    private readonly IMediator _mediator;

    public ReviewService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<GetHistoryByTeamResult> GetHistory(Guid teamId, DateOnly from, CancellationToken token)
    {
        return await _mediator.Send(new GetHistoryByTeamQuery(teamId, from), token);
    }

    public async Task<GetAverageByTeamResult> GetAverage(Guid teamId, DateOnly from, CancellationToken token)
    {
        return await _mediator.Send(new GetAverageByTeamQuery(teamId, from), token);
    }

    public async Task<GetLastTasksResult> GetLast(Guid teamId, DateOnly from, CancellationToken token)
    {
        return await _mediator.Send(new GetLastTasksQuery(teamId, from), token);
    }
}