using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Holidays.Model;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateCalendar;

internal sealed class UpdateCalendarCommandHandler : IRequestHandler<UpdateCalendarCommand, UpdateCalendarResult>
{
    private readonly ICalendarRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IHolidayReader _reader;

    public UpdateCalendarCommandHandler(
        ICalendarRepository repository,
        IPersonResolver personResolver,
        IHolidayReader reader)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<UpdateCalendarResult> Handle(UpdateCalendarCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var schedule = command.Schedule is null
            ? null
            : new WorkScheduleUtc(command.Schedule.Start, command.Schedule.End);
        var currentPerson = _personResolver.GetCurrentPerson();
        var existCalendar = await _repository.Find(currentPerson.Id, token);
        var calendar = (existCalendar?.Clear() ?? Calendar.Create(Guid.NewGuid(), currentPerson.Id))
            .SetSchedule(schedule);
        
        foreach (var weekend in command.Weekends)
            calendar = calendar.AddWeekend(weekend);

        foreach (var holiday in command.Holidays)
        {
            var holidayType = Enum.Parse<HolidayType>(holiday.Value);
            
            calendar = calendar.AddHoliday(holiday.Key, holidayType);
        }

        await _repository.Upsert(calendar, token);
        
        var botIds = await _repository.GetBotIds(calendar.Id, token);
        foreach (var botId in botIds)
            await _reader.Reload(botId, token);

        return new(calendar.Id);
    }
}