namespace Inc.TeamAssistant.Primitives.Extensions;

public static class TimeSpanExtensions
{
    public static string ToTime(this TimeSpan value)
    {
        const string timeFormat = @"d\.hh\:mm\:ss";
        
        var result = value.ToString(timeFormat);
        
        return result;
    }
}