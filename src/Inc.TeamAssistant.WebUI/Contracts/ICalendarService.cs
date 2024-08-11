using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ICalendarService
{
    Task<ServiceResult<GetCalendarByOwnerResult?>> GetCalendarByOwner(long ownerId, CancellationToken token = default);
}