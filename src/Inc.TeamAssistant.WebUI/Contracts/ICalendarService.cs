using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ICalendarService
{
    Task<ServiceResult<GetCalendarByOwnerResult?>> GetCalendarByOwner(CancellationToken token = default);

    Task<ServiceResult<Guid>> Create(CreateCalendarCommand command, CancellationToken token = default);

    Task<ServiceResult<Guid>> Update(UpdateCalendarCommand command, CancellationToken token = default);
}