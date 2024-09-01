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

	public async Task<GetActiveStoryResult> GetActiveStory(
		Guid teamId,
		string foreground,
		string background,
		CancellationToken token)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
		ArgumentException.ThrowIfNullOrWhiteSpace(background);
		
		return await _mediator.Send(new GetActiveStoryQuery(teamId, foreground, background), token);
	}

	public async Task<GetAssessmentHistoryResult> GetAssessmentHistory(
		Guid teamId,
		DateOnly? from,
		CancellationToken token)
	{
		return await _mediator.Send(new GetAssessmentHistoryQuery(teamId, from), token);
	}

	public async Task<GetStoriesResult> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token)
	{
		return await _mediator.Send(new GetStoriesQuery(teamId, assessmentDate), token);
	}
}