using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class ReviewClient : IReviewService
{
    private readonly HttpClient _client;

    public ReviewClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<GetHistoryByTeamResult> GetHistory(Guid teamId, DateOnly from, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetHistoryByTeamResult>(
            $"reviewer/history/{teamId:N}/{from:yyyy-MM-dd}",
            token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetAverageByTeamResult> GetAverage(Guid teamId, DateOnly from, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetAverageByTeamResult>(
            $"reviewer/average/{teamId:N}/{from:yyyy-MM-dd}",
            token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetLastTasksResult> GetLast(Guid teamId, DateOnly from, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetLastTasksResult>(
            $"reviewer/last/{teamId:N}/{from:yyyy-MM-dd}",
            token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}