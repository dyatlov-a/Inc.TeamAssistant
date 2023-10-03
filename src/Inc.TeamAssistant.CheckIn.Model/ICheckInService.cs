using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

namespace Inc.TeamAssistant.CheckIn.Model;

public interface ICheckInService
{
    Task<ServiceResult<GetLocationsResult?>> GetLocations(Guid mapId, CancellationToken cancellationToken = default);
}