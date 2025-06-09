using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class TenantClient : ITenantService
{
    private readonly HttpClient _client;

    public TenantClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetAvailableRoomsResult> GetAvailableRooms(Guid? id, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetAvailableRoomsResult>($"tenants/teams/available/{id:N}", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetRoomResult> GetRoom(Guid id, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRoomResult>($"tenants/teams/{id:N}", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<CreateRoomResult> CreateRoom(CreateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("tenants/teams", command, token);
        
        var result = await response.Content.ReadFromJsonAsync<CreateRoomResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task UpdateRoom(UpdateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("tenants/teams", command, token);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRoom(Guid teamId, CancellationToken token)
    {
        var response = await _client.DeleteAsync($"tenants/teams/{teamId:N}", token);
        
        response.EnsureSuccessStatusCode();
    }
}