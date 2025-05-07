using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
using Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class RetroClient : IRetroService
{
    private readonly HttpClient _client;

    public RetroClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetTeamResult> GetTeam(
        Guid id,
        CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetTeamResult>($"retro/team/{id:N}", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<CreateTeamResult> CreateTeam(
        CreateTeamCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("retro/team", command, token);
        
        var result = await response.Content.ReadFromJsonAsync<CreateTeamResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task UpdateTeam(
        UpdateTeamCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("retro/team", command, token);
        
        response.EnsureSuccessStatusCode();
    }
}