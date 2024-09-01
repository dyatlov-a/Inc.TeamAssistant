using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ICalendarService
{
    Task<GetCalendarByOwnerResult?> GetCalendarByOwner(CancellationToken token = default);

    Task<Guid> Create(CreateCalendarCommand command, CancellationToken token = default);

    Task<Guid> Update(UpdateCalendarCommand command, CancellationToken token = default);
}