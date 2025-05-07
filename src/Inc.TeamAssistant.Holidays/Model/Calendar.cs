namespace Inc.TeamAssistant.Holidays.Model;

public sealed record Calendar(
    Guid Id,
    long OwnerId,
    WorkScheduleUtc? Schedule,
    IReadOnlyCollection<DayOfWeek> Weekends,
    IReadOnlyDictionary<DateOnly, HolidayType> Holidays)
{
    public static Calendar Create(Guid id, long ownerId)
    {
        return new Calendar(
            id,
            ownerId,
            Schedule: null,
            Weekends: [],
            Holidays: new Dictionary<DateOnly, HolidayType>());
    }

    public Calendar SetSchedule(WorkScheduleUtc? schedule)
    {
        return this with
        {
            Schedule = schedule
        };
    }

    public Calendar Clear()
    {
        return this with
        {
            Weekends = [],
            Holidays = new Dictionary<DateOnly, HolidayType>()
        };
    }
    
    public Calendar AddWeekend(DayOfWeek dayOfWeek)
    {
        return this with
        {
            Weekends = Weekends.Contains(dayOfWeek)
                ? Weekends
                : Weekends.Append(dayOfWeek).ToArray()
        };
    }

    public Calendar AddHoliday(DateOnly date, HolidayType holidayType)
    {
        return this with
        {
            Holidays = new Dictionary<DateOnly, HolidayType>(Holidays)
            {
                [date] = holidayType
            }
        };
    }
    
    public (DateTimeOffset Start, DateTimeOffset End)? GetWorkTime(DateOnly date)
    {
        if (Schedule is null)
        {
            var startDefault = new DateTimeOffset(date, TimeOnly.MinValue, TimeSpan.Zero);
            var endDefault = new DateTimeOffset(date, TimeOnly.MaxValue, TimeSpan.Zero);

            return (startDefault, endDefault);
        }

        if (!IsWorkday(date))
            return null;

        var startByCalendar = new DateTimeOffset(date, Schedule.Start, TimeSpan.Zero);
        var endByCalendar = new DateTimeOffset(date, Schedule.End, TimeSpan.Zero);

        return (startByCalendar, endByCalendar);
    }
    
    public bool IsWorkTime(DateTimeOffset value)
    {
        if (Schedule is null)
            return true;
        
        return Schedule.IsWorkTime(value) && IsWorkday(DateOnly.FromDateTime(value.DateTime));
    }
    
    private bool IsWorkday(DateOnly date)
    {
        if (Holidays.TryGetValue(date, out var holidayType))
            return holidayType == HolidayType.Workday;

        return !Weekends.Contains(date.DayOfWeek);
    }
}