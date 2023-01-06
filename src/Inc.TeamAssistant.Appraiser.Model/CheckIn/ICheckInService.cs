using Inc.TeamAssistant.Appraiser.Model.CheckIn.Queries.GetLocations;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Model.CheckIn;

public interface ICheckInService
{
    Task<ServiceResult<GetLocationsResult?>> GetLocations(Guid mapId, CancellationToken cancellationToken = default);
}