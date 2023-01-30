using System.Net.Http.Json;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.CheckIn.Client;

public sealed class CheckInService : ICheckInService
{
    private readonly HttpClient _httpClient;

    public CheckInService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ServiceResult<AddLocationToMapResult>> AddLocationToMap(
        AddLocationToMapCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = await _httpClient.PostAsJsonAsync("locations", command, cancellationToken);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadFromJsonAsync<ServiceResult<AddLocationToMapResult>>(
                cancellationToken: cancellationToken);
            return response!;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<AddLocationToMapResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetLocationsResult>> GetLocations(Guid mapId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResult<GetLocationsResult>>(
                $"locations/{mapId}",
                cancellationToken);
            return response!;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLocationsResult>(ex.Message);
        }
    }
}