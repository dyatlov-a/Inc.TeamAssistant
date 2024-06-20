using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ICheckInService
{
    Task<ServiceResult<GetMapsResult>> GetMaps(Guid botId, CancellationToken token = default);
    
    Task<ServiceResult<GetLocationsResult?>> GetLocations(Guid mapId, CancellationToken token = default);
}