namespace Inc.TeamAssistant.Holidays.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset GetLastDayOfWeek(this DateTimeOffset date, DayOfWeek dayOfWeek)
    {
        var currentDate = date;
        
        while (currentDate.DayOfWeek != dayOfWeek)
            currentDate = currentDate.AddDays(-1);

        return currentDate;
    }
}