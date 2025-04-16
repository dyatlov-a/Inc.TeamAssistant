using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class AppraiserClient : IAppraiserService
{
	private readonly HttpClient _client;

    public AppraiserClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetActiveStoryResult> GetActiveStory(
	    Guid teamId,
	    string foreground,
	    string background,
	    CancellationToken token)
    {
	    ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
	    ArgumentException.ThrowIfNullOrWhiteSpace(background);
	    
	    var result = await _client.GetFromJsonAsync<GetActiveStoryResult>(
		    $"assessment-sessions/story/{teamId}/active?foreground={foreground}&background={background}",
		    token);

	    if (result is null)
		    throw new TeamAssistantException("Parse response with error.");

	    return result;
    }

	public async Task<GetAssessmentHistoryResult> GetAssessmentHistory(
		Guid teamId,
		int? limit,
		DateOnly? from,
		CancellationToken token)
	{
		var result = await _client.GetFromJsonAsync<GetAssessmentHistoryResult>(
			$"assessment-sessions/history?teamid={teamId}&limit={limit}&from={from:yyyy-MM-dd}",
			token);

		if (result is null)
			throw new TeamAssistantException("Parse response with error.");

		return result;
	}

	public async Task<GetStoriesResult> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token)
	{
		var result = await _client.GetFromJsonAsync<GetStoriesResult>(
			$"assessment-sessions/stories/{teamId}/{assessmentDate:yyyy-MM-dd}",
			token);

		if (result is null)
			throw new TeamAssistantException("Parse response with error.");

		return result;
	}
}