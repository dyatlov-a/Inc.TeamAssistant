using System.Net.Http.Json;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class CheckInClient : ICheckInService
{
    private readonly HttpClient _client;

    public CheckInClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetMapsResult> GetMaps(Guid botId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetMapsResult>($"check-in/maps/{botId:N}", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetLocationsResult> GetLocations(Guid mapId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetLocationsResult>($"check-in/locations/{mapId:N}", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}