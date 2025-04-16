using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class RandomCoffeeClient : IRandomCoffeeService
{
    private readonly HttpClient _client;

    public RandomCoffeeClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<GetChatsResult> GetChatsByBot(Guid botId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetChatsResult>($"random-coffee/chats/{botId:N}", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetHistoryResult> GetHistory(Guid botId, long chatId, int depth, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetHistoryResult>(
            $"random-coffee/history?botid={botId:N}&chatid={chatId}&depth={depth}",
            token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}