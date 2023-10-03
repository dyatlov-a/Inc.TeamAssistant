using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.CheckIn.Model;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

namespace Inc.TeamAssistant.WebUI.Services.CheckIn;

internal sealed class CheckInClient : ICheckInService
{
    private readonly HttpClient _client;

    public CheckInClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<ServiceResult<GetLocationsResult?>> GetLocations(Guid mapId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetLocationsResult?>>(
                $"check-in/{mapId}",
                cancellationToken);

            if (result is null)
                throw new ApplicationException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLocationsResult?>(ex.Message);
        }
    }
}