using Inc.TeamAssistant.Appraiser.Model.Common;
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

    public async Task<ServiceResult<GetHistoryByTeamResult>> GetHistory(
        Guid teamId,
        DateOnly from,
        CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetHistoryByTeamQuery(teamId, from), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetHistoryByTeamResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetAverageByTeamResult>> GetAverage(
        Guid teamId,
        DateOnly from,
        CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetAverageByTeamQuery(teamId, from), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetAverageByTeamResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetLastTasksResult>> GetLast(Guid teamId, int count, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetLastTasksQuery(teamId, count), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLastTasksResult>(ex.Message);
        }
    }
}