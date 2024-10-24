using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ICheckInService
{
    Task<GetMapsResult> GetMaps(Guid botId, CancellationToken token = default);
    
    Task<GetLocationsResult> GetLocations(Guid mapId, CancellationToken token = default);
}