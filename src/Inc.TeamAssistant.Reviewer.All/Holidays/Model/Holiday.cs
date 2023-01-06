namespace Inc.TeamAssistant.Reviewer.All.Holidays.Model;

public sealed class Holiday
{
    public DateOnly Date { get; private set; }
    public HolidayType Type { get; private set; }

    private Holiday()
    {
    }

    public Holiday(DateOnly date, HolidayType type)
        : this()
    {
        Date = date;
        Type = type;
    }
}