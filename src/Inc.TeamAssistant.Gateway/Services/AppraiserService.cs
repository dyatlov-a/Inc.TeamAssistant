using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryById;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class AppraiserService : IAppraiserService
{
	private readonly IMediator _mediator;

	public AppraiserService(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        Guid teamId,
		CancellationToken token)
	{
		try
		{
			var result = await _mediator.Send(new GetStoryDetailsQuery(teamId), token);

			return result is null
				? ServiceResult.NotFound<GetStoryDetailsResult?>()
				: ServiceResult.Success((GetStoryDetailsResult?)result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoryDetailsResult?>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetAssessmentHistoryResult?>> GetAssessmentHistory(Guid teamId, int depth, CancellationToken token = default)
	{
		try
		{
			var result = await _mediator.Send(new GetAssessmentHistoryQuery(teamId, depth), token);

			return ServiceResult.Success((GetAssessmentHistoryResult?)result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetAssessmentHistoryResult?>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetStoriesResult?>> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token = default)
	{
		try
		{
			var result = await _mediator.Send(new GetStoriesQuery(teamId, assessmentDate), token);

			return ServiceResult.Success((GetStoriesResult?)result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoriesResult?>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetStoryByIdResult?>> GetStoryById(Guid storyId, CancellationToken token = default)
	{
		try
		{
			var result = await _mediator.Send(new GetStoryByIdQuery(storyId), token);

			return result is null
				? ServiceResult.NotFound<GetStoryByIdResult?>()
				: ServiceResult.Success((GetStoryByIdResult?)result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoryByIdResult?>(ex.Message);
		}
	}
}