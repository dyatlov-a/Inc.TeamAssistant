namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CalendarFormModel
{
    public bool WorkAllDay { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public List<DayOfWeek> SelectedWeekends { get; set; } = default!;
    public List<HolidayFromModel> Holidays { get; set; } = default!;
    
    public CalendarFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);
        
        WorkAllDay = stagesState.Calendar.WorkAllDay;
        Start = stagesState.Calendar.Schedule.Start;
        End = stagesState.Calendar.Schedule.End;
        SelectedWeekends = stagesState.Calendar.Weekends.ToList();
        Holidays = stagesState.Calendar.Holidays.Select(i => new HolidayFromModel
        {
            Date = i.Key,
            IsWorkday = i.Value.Equals("Workday", StringComparison.InvariantCultureIgnoreCase)
        }).ToList();
        
        return this;
    }
}