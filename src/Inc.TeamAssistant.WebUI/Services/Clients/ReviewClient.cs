using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class ReviewClient : IReviewService
{
    private readonly HttpClient _client;

    public ReviewClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<ServiceResult<GetHistoryByTeamResult>> GetHistory(
        Guid teamId,
        DateOnly from,
        CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetHistoryByTeamResult>>(
                $"reviewer/history/{teamId:N}/{from:yyyy-MM-dd}",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetHistoryByTeamResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetAverageByTeamResult>> GetAverage(
        Guid teamId,
        DateOnly from,
        CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetAverageByTeamResult>>(
                $"reviewer/average/{teamId:N}/{from:yyyy-MM-dd}",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetAverageByTeamResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetLastTasksResult>> GetLast(Guid teamId, DateOnly from, CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetLastTasksResult>>(
                $"reviewer/last/{teamId:N}/{from:yyyy-MM-dd}",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLastTasksResult>(ex.Message);
        }
    }
}