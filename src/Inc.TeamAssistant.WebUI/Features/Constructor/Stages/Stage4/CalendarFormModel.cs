namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CalendarFormModel
{
    public bool WorkAllDay { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public List<DayOfWeek> SelectedWeekends { get; } = new();
    public List<HolidayFromModel> Holidays { get; } = new();
    
    public CalendarFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);
        
        WorkAllDay = stagesState.Calendar.WorkAllDay;
        Start = stagesState.Calendar.Schedule.Start;
        End = stagesState.Calendar.Schedule.End;
        
        SelectedWeekends.Clear();
        SelectedWeekends.AddRange(SelectedWeekends);
        
        Holidays.Clear();
        foreach (var holiday in stagesState.Calendar.Holidays)
            AddHoliday(holiday.Key, holiday.Value.Equals("Workday", StringComparison.InvariantCultureIgnoreCase));
        
        return this;
    }

    public void AddHoliday(DateOnly date, bool isWorkday)
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
}