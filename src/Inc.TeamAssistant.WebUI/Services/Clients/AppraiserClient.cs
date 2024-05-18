using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class AppraiserClient : IAppraiserService
{
	private readonly HttpClient _client;

    public AppraiserClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<ServiceResult<GetActiveStoryResult>> GetActiveStory(Guid teamId, CancellationToken token)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetActiveStoryResult>>(
				$"assessment-sessions/story/{teamId}/active",
				token);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetActiveStoryResult>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetAssessmentHistoryResult>> GetAssessmentHistory(
		Guid teamId,
		int depth,
		CancellationToken token)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetAssessmentHistoryResult>>(
				$"assessment-sessions/history?teamid={teamId}&depth={depth}",
				token);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
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
			var result = await _client.GetFromJsonAsync<ServiceResult<GetStoriesResult>>(
				$"assessment-sessions/stories/{teamId}/{assessmentDate:yyyy-MM-dd}",
				token);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoriesResult>(ex.Message);
		}
	}
}