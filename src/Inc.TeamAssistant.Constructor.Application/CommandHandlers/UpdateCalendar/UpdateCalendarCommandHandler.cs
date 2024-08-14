using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Holidays.Model;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateCalendar;

internal sealed class UpdateCalendarCommandHandler : IRequestHandler<UpdateCalendarCommand, Guid>
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;
    private readonly IHolidayReader _holidayReader;

    public UpdateCalendarCommandHandler(
        ICalendarRepository calendarRepository,
        ICurrentPersonResolver currentPersonResolver,
        IHolidayReader holidayReader)
    {
        _calendarRepository = calendarRepository ?? throw new ArgumentNullException(nameof(calendarRepository));
        _currentPersonResolver =
            currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
        _holidayReader = holidayReader ?? throw new ArgumentNullException(nameof(holidayReader));
    }

    public async Task<Guid> Handle(UpdateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        var calendar = await _calendarRepository.GetById(command.Id, token);
        if (calendar.OwnerId != currentPerson.Id)
            throw new ApplicationException($"User {currentPerson.Id} has not access to calendar {command.Id}.");

        calendar
            .Clear()
            .SetSchedule(command.Schedule is null
                ? null
                : new WorkScheduleUtc(command.Schedule.Start, command.Schedule.End));
        
        foreach (var weekend in command.Weekends)
            calendar.AddWeekend(weekend);
        
        foreach (var holiday in command.Holidays)
            calendar.AddHoliday(holiday.Key, Enum.Parse<HolidayType>(holiday.Value));

        await _calendarRepository.Upsert(calendar, token);
        
        var botIds = await _calendarRepository.GetBotIds(calendar.Id, token);
        foreach (var botId in botIds)
            _holidayReader.Reload(botId);

        return calendar.Id;
    }
}