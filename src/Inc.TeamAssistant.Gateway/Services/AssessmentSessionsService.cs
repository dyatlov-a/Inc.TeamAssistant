using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class AssessmentSessionsService : IAssessmentSessionsService
{
	private readonly IMediator _mediator;

	public AssessmentSessionsService(IMediator mediator)
		=> _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

	public async Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        Guid assessmentSessionId,
		CancellationToken cancellationToken)
	{
		try
		{
			var result = await _mediator.Send(
                new GetStoryDetailsQuery(assessmentSessionId),
                cancellationToken);

			return result is null
				? ServiceResult.NotFound<GetStoryDetailsResult?>()
				: ServiceResult.Success((GetStoryDetailsResult?)result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoryDetailsResult?>(ex.Message);
		}
	}

    public async Task<ServiceResult<GetLinkForConnectResult>> GetLinkForConnect(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(
                new GetLinkForConnectQuery(),
                cancellationToken);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLinkForConnectResult>(ex.Message);
        }
    }
}