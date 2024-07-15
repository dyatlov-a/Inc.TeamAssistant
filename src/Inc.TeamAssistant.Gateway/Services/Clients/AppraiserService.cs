using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class AppraiserService : IAppraiserService
{
	private readonly IMediator _mediator;

	public AppraiserService(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<ServiceResult<GetActiveStoryResult>> GetActiveStory(
		Guid teamId,
		string foreground,
		string background,
		CancellationToken token)
	{
		if (string.IsNullOrWhiteSpace(foreground))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(foreground));
		if (string.IsNullOrWhiteSpace(background))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(background));
		
		try
		{
			var result = await _mediator.Send(new GetActiveStoryQuery(teamId, foreground, background), token);

			return ServiceResult.Success(result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetActiveStoryResult>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetAssessmentHistoryResult>> GetAssessmentHistory(
		Guid teamId,
		DateOnly? from,
		CancellationToken token)
	{
		try
		{
			var result = await _mediator.Send(new GetAssessmentHistoryQuery(teamId, from), token);

			return ServiceResult.Success(result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetAssessmentHistoryResult>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetStoriesResult>> GetStories(
		Guid teamId,
		DateOnly assessmentDate,
		CancellationToken token)
	{
		try
		{
			var result = await _mediator.Send(new GetStoriesQuery(teamId, assessmentDate), token);

			return ServiceResult.Success(result);
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoriesResult>(ex.Message);
		}
	}
}