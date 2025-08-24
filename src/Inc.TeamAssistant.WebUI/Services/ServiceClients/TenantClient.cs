using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class TenantClient : ITenantService
{
    private readonly HttpClient _client;

    public TenantClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetAvailableRoomsResult> GetAvailableRooms(Guid? roomId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetAvailableRoomsResult>($"tenants/rooms/available/{roomId:N}", token);

        return result ?? throw new TeamAssistantException("Parse response with error.");
    }

    public async Task<GetRoomResult> GetRoom(Guid roomId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRoomResult>($"tenants/rooms/{roomId:N}", token);

        return result ?? throw new TeamAssistantException("Parse response with error.");
    }

    public async Task<GetRoomPropertiesResult> GetRoomProperties(Guid roomId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRoomPropertiesResult>(
            $"tenants/rooms/{roomId:N}/properties",
            token);

        return result ?? throw new TeamAssistantException("Parse response with error.");
    }

    public async Task<GetRoomHistoryResult> GetRoomHistory(Guid roomId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRoomHistoryResult>(
            $"tenants/rooms/{roomId:N}/history",
            token);

        return result ?? throw new TeamAssistantException("Parse response with error.");
    }

    public async Task ChangeRoomProperties(ChangeRoomPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("tenants/rooms/properties", command, token);

        await response.HandleValidation(token);
    }

    public async Task<CreateRoomResult> CreateRoom(CreateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("tenants/rooms", command, token);
        
        var result = await response.Content.ReadFromJsonAsync<CreateRoomResult>(token);
        
        return result ?? throw new TeamAssistantException("Parse response with error.");
    }

    public async Task UpdateRoom(UpdateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("tenants/rooms", command, token);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRoom(Guid roomId, CancellationToken token)
    {
        var response = await _client.DeleteAsync($"tenants/rooms/{roomId:N}", token);
        
        response.EnsureSuccessStatusCode();
    }
}