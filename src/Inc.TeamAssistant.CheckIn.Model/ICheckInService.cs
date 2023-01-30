using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.CheckIn.Model;

public interface ICheckInService
{
    Task<ServiceResult<AddLocationToMapResult>> AddLocationToMap(
        AddLocationToMapCommand command,
        CancellationToken cancellationToken = default); 

    Task<ServiceResult<GetLocationsResult>> GetLocations(Guid mapId, CancellationToken cancellationToken = default);
}