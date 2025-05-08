using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableTeams;
using Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class TenantClient : ITenantService
{
    private readonly HttpClient _client;

    public TenantClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetAvailableTeamsResult> GetAvailableTeams(Guid? id, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetAvailableTeamsResult>($"tenants/teams/available/{id:N}", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetTeamResult> GetTeam(Guid id, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetTeamResult>($"tenants/teams/{id:N}", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<CreateTeamResult> CreateTeam(CreateTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("tenants/teams", command, token);
        
        var result = await response.Content.ReadFromJsonAsync<CreateTeamResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task UpdateTeam(UpdateTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("tenants/teams", command, token);
        
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveTeam(Guid teamId, CancellationToken token)
    {
        var response = await _client.DeleteAsync($"tenants/teams/{teamId:N}", token);
        
        response.EnsureSuccessStatusCode();
    }
}