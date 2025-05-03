using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class UserClient : IUserService
{
    private readonly HttpClient _client;

    public UserClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<BotContext> GetAuthBotContext(CancellationToken token = default)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<BotContext>("accounts/bot", token);
            
            return result ?? BotContext.Empty;
        }
        catch
        {
            return BotContext.Empty;
        }
    }
}