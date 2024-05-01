using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryById;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
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

	public async Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        Guid teamId,
		CancellationToken token)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetStoryDetailsResult?>>(
				$"assessment-sessions/story/{teamId}/current",
				token);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoryDetailsResult?>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetAssessmentHistoryResult?>> GetAssessmentHistory(
		Guid teamId,
		int depth,
		CancellationToken token)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetAssessmentHistoryResult?>>(
				$"assessment-sessions/history?teamid={teamId}&depth={depth}",
				token);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetAssessmentHistoryResult?>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetStoriesResult?>> GetStories(
		Guid teamId,
		DateOnly assessmentDate,
		CancellationToken token)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetStoriesResult?>>(
				$"assessment-sessions/stories/{teamId}/{assessmentDate:yyyy-MM-dd}",
				token);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoriesResult?>(ex.Message);
		}
	}

	public async Task<ServiceResult<GetStoryByIdResult?>> GetStoryById(Guid storyId, CancellationToken token)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetStoryByIdResult?>>(
				$"assessment-sessions/story/{storyId}",
				token);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoryByIdResult?>(ex.Message);
		}
	}
}