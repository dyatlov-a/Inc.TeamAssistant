using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class RetroClient : IRetroService
{
    private readonly HttpClient _client;

    public RetroClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetRetroItemsResult> GetItems(Guid teamId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRetroItemsResult>($"retro/items/{teamId:N}", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
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

    public async Task UpdateRetroItem(UpdateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("retro/items", command, token);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRetroItem(Guid retroItemId, CancellationToken token)
    {
        var response = await _client.DeleteAsync($"retro/items/{retroItemId:N}", token);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task<StartRetroResult> StartRetro(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("retro", command, token);
        
        var result = await response.Content.ReadFromJsonAsync<StartRetroResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}