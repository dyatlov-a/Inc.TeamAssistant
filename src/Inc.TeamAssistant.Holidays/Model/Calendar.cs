namespace Inc.TeamAssistant.Holidays.Model;

public sealed class Calendar
{
    public Guid Id { get; private set; }
    public long OwnerId { get; private set; }
    public WorkScheduleUtc? Schedule { get; private set; }
    public IReadOnlyCollection<DayOfWeek> Weekends { get; private set; } = [];
    public IReadOnlyDictionary<DateOnly, HolidayType> Holidays { get; private set; }
        = new Dictionary<DateOnly, HolidayType>();

    private Calendar()
    {
    }

    public Calendar(Guid id, long ownerId)
        : this()
    {
        Id = id;
        OwnerId = ownerId;
    }

    public Calendar SetSchedule(WorkScheduleUtc? schedule)
    {
        Schedule = schedule;
        
        return this;
    }

    public Calendar Clear()
    {
        Weekends = [];
        Holidays = new Dictionary<DateOnly, HolidayType>();
        
        return this;
    }
    
    public Calendar AddWeekend(DayOfWeek dayOfWeek)
    {
        if (!Weekends.Contains(dayOfWeek))
            Weekends = Weekends.Append(dayOfWeek).ToArray();
        
        return this;
    }

    public Calendar AddHoliday(DateOnly date, HolidayType holidayType)
    {
        Holidays = new Dictionary<DateOnly, HolidayType>(Holidays)
        {
            [date] = holidayType
        };

        return this;
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