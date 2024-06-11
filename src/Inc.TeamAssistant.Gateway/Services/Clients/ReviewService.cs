using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
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

    public async Task<ServiceResult<GetHistoryByTeamResult>> GetHistory(Guid teamId, int depth, CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetHistoryByTeamQuery(teamId, depth), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetHistoryByTeamResult>(ex.Message);
        }
    }
}