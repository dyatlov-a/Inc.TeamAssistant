using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class RandomCoffeeClient : IRandomCoffeeService
{
    private readonly HttpClient _client;

    public RandomCoffeeClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<ServiceResult<GetChatsResult>> GetChatsByBot(Guid botId, CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetChatsResult>>(
                $"random-coffee/chats/{botId:N}",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetChatsResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetHistoryResult>> GetHistory(
        Guid botId,
        long chatId,
        int depth,
        CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetHistoryResult>>(
                $"random-coffee/history?botid={botId:N}&chatid={chatId}&depth={depth}",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetHistoryResult>(ex.Message);
        }
    }
}