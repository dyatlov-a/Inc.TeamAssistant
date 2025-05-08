using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class RetroClient : IRetroService
{
    private readonly HttpClient _client;

    public RetroClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<CreateRetroItemResult> CreateRetroItem(CreateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("retro/items", command, token);
        
        var result = await response.Content.ReadFromJsonAsync<CreateRetroItemResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}