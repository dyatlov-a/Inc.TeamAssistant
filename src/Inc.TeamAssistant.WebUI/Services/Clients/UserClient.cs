using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class UserClient : IUserService
{
    private readonly HttpClient _client;

    public UserClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<Person?> GetCurrentUser(CancellationToken token)
    {
        try
        {
            return await _client.GetFromJsonAsync<Person?>("accounts/current-user", token);
        }
        catch
        {
            return null;
        }
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