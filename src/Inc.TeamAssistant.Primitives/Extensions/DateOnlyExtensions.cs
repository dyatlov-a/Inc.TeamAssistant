namespace Inc.TeamAssistant.Primitives.Extensions;

public static class DateOnlyExtensions
{
    public static DateTimeOffset ToDateTimeOffset(this DateOnly value) => new(value, TimeOnly.MinValue, TimeSpan.Zero);

    public static DateOnly ToDateOnly(this DateTimeOffset value) => new(value.Year, value.Month, value.Day);
}