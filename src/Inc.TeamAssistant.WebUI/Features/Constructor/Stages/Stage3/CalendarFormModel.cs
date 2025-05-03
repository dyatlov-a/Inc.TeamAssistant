using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class CalendarFormModel
{
    public bool WorkAllDay { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    
    private readonly List<DayOfWeek> _workdays = new();
    public IReadOnlyCollection<DayOfWeek> Workdays => _workdays;
    
    private readonly List<HolidayFromModel> _holidays = new();
    public IReadOnlyCollection<HolidayFromModel> Holidays => _holidays;

    public IEnumerable<DayOfWeek> WeekDays
    {
        get
        {
            yield return DayOfWeek.Monday;
            yield return DayOfWeek.Tuesday;
            yield return DayOfWeek.Wednesday;
            yield return DayOfWeek.Thursday;
            yield return DayOfWeek.Friday;
            yield return DayOfWeek.Saturday;
            yield return DayOfWeek.Sunday;
        }
    }
    
    public CalendarFormModel Apply(CalendarViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        WorkAllDay = viewModel.Calendar.Schedule is null;
        (Start, End) = viewModel.Calendar.Schedule is null
            ? CreateDefaultTime(viewModel.ClientTimezoneOffset)
            : (viewModel.Calendar.Schedule.Start, viewModel.Calendar.Schedule.End);
        
        _workdays.Clear();
        _workdays.AddRange(WeekDays.Except(viewModel.Calendar.Weekends));
        
        _holidays.Clear();
        foreach (var holiday in viewModel.Calendar.Holidays)
            AddHoliday(holiday.Key, holiday.Value.Equals("Workday", StringComparison.InvariantCultureIgnoreCase));

        return this;
    }
    
    public CalendarFormModel Apply(int clientTimezoneOffset)
    {
        WorkAllDay = false;
        (Start, End) = CreateDefaultTime(clientTimezoneOffset);
        
        _workdays.Clear();
        _workdays.AddRange([
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        ]);
        
        _holidays.Clear();
        
        return this;
    }
    
    public void AddHoliday()
    {
        var now = DateTimeOffset.UtcNow;
        var date = Holidays.Any()
            ? Holidays.OrderByDescending(h => h.Date).First().Date
            : new DateOnly(now.Year, now.Month, now.Day);
        
        AddHoliday(date.AddDays(1), isWorkday: false);
    }

    public void RemoveHoliday(HolidayFromModel holiday)
    {
        ArgumentNullException.ThrowIfNull(holiday);

        _holidays.Remove(holiday);
    }
    
    public void SetWorkdays(IEnumerable<DayOfWeek> items)
    {
        _workdays.Clear();
        _workdays.AddRange(items);
    }

    public UpdateCalendarCommand ToCommand()
    {
        return new UpdateCalendarCommand(
            ToWorkScheduleUtcDto(),
            WeekDays.Except(_workdays).ToArray(),
            ToHolidays());
    }

    private WorkScheduleUtcDto? ToWorkScheduleUtcDto() => WorkAllDay ? null : new WorkScheduleUtcDto(Start, End);

    private IReadOnlyDictionary<DateOnly, string> ToHolidays() => Holidays.ToDictionary(
        h => h.Date,
        h => h.IsWorkday ? "Workday" : "Holiday");

    private void AddHoliday(DateOnly date, bool isWorkday)
    {
        var holidaysCount = _holidays.Count;
        _holidays.Add(new HolidayFromModel
        {
            DateFieldId = $"date-{holidaysCount}",
            WorkdayFieldId = $"workday-{holidaysCount}",
            Date = date,
            IsWorkday = isWorkday
        });
    }
    
    private (TimeOnly Start, TimeOnly End) CreateDefaultTime(int clientTimezoneOffset)
    {
        const int startMinutesUtc = 10 * 60;
        const int endMinutesUtc = 19 * 60;
        
        var start = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(startMinutesUtc + clientTimezoneOffset));
        var end = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(endMinutesUtc + clientTimezoneOffset));
        
        return (start, end);
    }
}