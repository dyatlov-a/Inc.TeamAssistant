using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class MessageProviderClient : IMessageProvider
{
    private readonly HttpClient _client;

    public MessageProviderClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Dictionary<string, Dictionary<string, string>>> Get(CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<Dictionary<string, Dictionary<string, string>>>(
            "resources",
            token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}