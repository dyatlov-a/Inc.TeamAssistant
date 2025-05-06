using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroCardPool;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroCardPool;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class RetroClient : IRetroService
{
    private readonly HttpClient _client;

    public RetroClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<CreateRetroCardPoolResult> CreateRetroCardPool(
        CreateRetroCardPoolCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("retro/card-pool", command, token);
        
        var result = await response.Content.ReadFromJsonAsync<CreateRetroCardPoolResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task UpdateRetroCardPool(
        UpdateRetroCardPoolCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("retro/card-pool", command, token);
        
        response.EnsureSuccessStatusCode();
    }
}