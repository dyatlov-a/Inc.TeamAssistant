using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;
using Inc.TeamAssistant.Holidays.Model;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateCalendar;

internal sealed class CreateCalendarCommandHandler : IRequestHandler<CreateCalendarCommand>
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;

    public CreateCalendarCommandHandler(
        ICalendarRepository calendarRepository,
        ICurrentPersonResolver currentPersonResolver)
    {
        _calendarRepository = calendarRepository ?? throw new ArgumentNullException(nameof(calendarRepository));
        _currentPersonResolver =
            currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
    }

    public async Task Handle(CreateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        var calendar = new Calendar(
            Guid.NewGuid(),
            currentPerson.Id,
            command.Schedule is null
            ? null
            : new WorkScheduleUtc(command.Schedule.Start, command.Schedule.End));

        foreach (var weekend in command.Weekends)
            calendar.AddWeekend(weekend);
        
        foreach (var holiday in command.Holidays)
            calendar.AddHoliday(holiday.Key, Enum.Parse<HolidayType>(holiday.Value));

        await _calendarRepository.Upsert(calendar, token);
    }
}