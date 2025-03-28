namespace Inc.TeamAssistant.Primitives.Extensions;

public static class DateOnlyExtensions
{
    public static DateTimeOffset ToDateTimeOffset(this DateOnly value) => new(value, TimeOnly.MinValue, TimeSpan.Zero);
}