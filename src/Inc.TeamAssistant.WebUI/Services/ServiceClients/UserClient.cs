using System.Net.Http.Json;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Features.Auth;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class UserClient : IUserService
{
    private readonly HttpClient _client;

    public UserClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<AuthBotContext> GetAuthBotContext(CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<AuthBotContext>("accounts/bot", token);
            
            return result ?? AuthBotContext.Empty;
        }
        catch
        {
            return AuthBotContext.Empty;
        }
    }
}