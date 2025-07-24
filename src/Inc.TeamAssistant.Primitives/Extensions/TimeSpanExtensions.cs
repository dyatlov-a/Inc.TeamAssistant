namespace Inc.TeamAssistant.Primitives.Extensions;

public static class TimeSpanExtensions
{
    public static string ToLongTime(this TimeSpan value)
    {
        const string timeFormat = @"d\.hh\:mm\:ss";
        
        var result = value.ToString(timeFormat);
        
        return result;
    }
    
    public static string ToShortTime(this TimeSpan value)
    {
        const string timeFormat = @"mm\:ss";
        
        var result = value.ToString(timeFormat);
        
        return result;
    }
    
    private static string ToTime(this TimeSpan value, string timeFormat) => value.ToString(timeFormat);
}