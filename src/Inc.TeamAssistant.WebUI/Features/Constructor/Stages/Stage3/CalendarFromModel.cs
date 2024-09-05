using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class CalendarFromModel
{
    public bool WorkAllDay { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public List<DayOfWeek> SelectedWeekends { get; } = new();
    public List<HolidayFromModel> Holidays { get; } = new();
    
    public CalendarFromModel Apply(GetCalendarByOwnerResult calendar, int clientTimezoneOffset)
    {
        ArgumentNullException.ThrowIfNull(calendar);

        WorkAllDay = calendar.Schedule is null;
        (Start, End) = calendar.Schedule is null
            ? CreateDefaultTime(clientTimezoneOffset)
            : (calendar.Schedule.Start, calendar.Schedule.End);
        
        SelectedWeekends.Clear();
        SelectedWeekends.AddRange(calendar.Weekends);
        
        Holidays.Clear();
        foreach (var holiday in calendar.Holidays)
            AddHoliday(holiday.Key, holiday.Value.Equals("Workday", StringComparison.InvariantCultureIgnoreCase));

        return this;
    }
    
    public CalendarFromModel Apply(int clientTimezoneOffset)
    {
        WorkAllDay = false;
        (Start, End) = CreateDefaultTime(clientTimezoneOffset);
        
        SelectedWeekends.Clear();
        SelectedWeekends.AddRange(new [] { DayOfWeek.Sunday, DayOfWeek.Saturday });
        
        Holidays.Clear();
        
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
    
    public void ToggleWeekend(DayOfWeek item)
    {
        if (SelectedWeekends.Contains(item))
            SelectedWeekends.Remove(item);
        else
            SelectedWeekends.Add(item);
    }

    public UpdateCalendarCommand ToCommand()
    {
        return new UpdateCalendarCommand(
            ToWorkScheduleUtcDto(),
            SelectedWeekends,
            ToHolidays());
    }

    private WorkScheduleUtcDto? ToWorkScheduleUtcDto() => WorkAllDay ? null : new WorkScheduleUtcDto(Start, End);

    private IReadOnlyDictionary<DateOnly, string> ToHolidays() => Holidays.ToDictionary(
        h => h.Date,
        h => h.IsWorkday ? "Workday" : "Holiday");

    private void AddHoliday(DateOnly date, bool isWorkday)
    {
        var holidaysCount = Holidays.Count;
        Holidays.Add(new HolidayFromModel
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