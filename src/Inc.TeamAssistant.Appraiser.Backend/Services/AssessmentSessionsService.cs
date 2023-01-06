using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

internal sealed class AssessmentSessionsService : IAssessmentSessionsService
{
	private readonly IMediator _mediator;

	public AssessmentSessionsService(IMediator mediator)
		=> _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

	public async Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        AssessmentSessionId assessmentSessionId,
        int width,
        int height,
        bool drawQuietZones,
		CancellationToken cancellationToken)
	{
		try
		{
			var result = await _mediator.Send(
                new GetStoryDetailsQuery(assessmentSessionId, width, height, drawQuietZones),
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

    public async Task<ServiceResult<GetLinkForConnectResult>> GetLinkForConnect(
        int width,
        int height,
        bool drawQuietZones,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(
                new GetLinkForConnectQuery(width, height, drawQuietZones),
                cancellationToken);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLinkForConnectResult>(ex.Message);
        }
    }
}