using Inc.TeamAssistant.Holidays.Extensions;
using Xunit;

namespace Inc.TeamAssistant.HolidaysTests.Extensions;

public sealed class DateTimeOffsetExtensionsTests
{
    [Theory]
    [InlineData(DayOfWeek.Monday, "2024-04-15T04:00:00+03:00", "2024-04-15T04:00:00+03:00")]
    [InlineData(DayOfWeek.Monday, "2024-04-16T04:00:00+03:00", "2024-04-15T04:00:00+03:00")]
    [InlineData(DayOfWeek.Monday, "2024-04-17T04:00:00+03:00", "2024-04-15T04:00:00+03:00")]
    [InlineData(DayOfWeek.Monday, "2024-04-18T04:00:00+03:00", "2024-04-15T04:00:00+03:00")]
    [InlineData(DayOfWeek.Monday, "2024-04-19T04:00:00+03:00", "2024-04-15T04:00:00+03:00")]
    [InlineData(DayOfWeek.Monday, "2024-04-20T04:00:00+03:00", "2024-04-15T04:00:00+03:00")]
    [InlineData(DayOfWeek.Monday, "2024-04-21T04:00:00+03:00", "2024-04-15T04:00:00+03:00")]
    [InlineData(DayOfWeek.Friday, "2024-04-15T04:00:00+03:00", "2024-04-12T04:00:00+03:00")]
    [InlineData(DayOfWeek.Friday, "2024-04-16T04:00:00+03:00", "2024-04-12T04:00:00+03:00")]
    [InlineData(DayOfWeek.Friday, "2024-04-17T04:00:00+03:00", "2024-04-12T04:00:00+03:00")]
    [InlineData(DayOfWeek.Friday, "2024-04-18T04:00:00+03:00", "2024-04-12T04:00:00+03:00")]
    [InlineData(DayOfWeek.Friday, "2024-04-19T04:00:00+03:00", "2024-04-19T04:00:00+03:00")]
    [InlineData(DayOfWeek.Friday, "2024-04-20T04:00:00+03:00", "2024-04-19T04:00:00+03:00")]
    [InlineData(DayOfWeek.Friday, "2024-04-21T04:00:00+03:00", "2024-04-19T04:00:00+03:00")]
    [InlineData(DayOfWeek.Sunday, "2024-04-15T04:00:00+03:00", "2024-04-14T04:00:00+03:00")]
    [InlineData(DayOfWeek.Sunday, "2024-04-16T04:00:00+03:00", "2024-04-14T04:00:00+03:00")]
    [InlineData(DayOfWeek.Sunday, "2024-04-17T04:00:00+03:00", "2024-04-14T04:00:00+03:00")]
    [InlineData(DayOfWeek.Sunday, "2024-04-18T04:00:00+03:00", "2024-04-14T04:00:00+03:00")]
    [InlineData(DayOfWeek.Sunday, "2024-04-19T04:00:00+03:00", "2024-04-14T04:00:00+03:00")]
    [InlineData(DayOfWeek.Sunday, "2024-04-20T04:00:00+03:00", "2024-04-14T04:00:00+03:00")]
    [InlineData(DayOfWeek.Sunday, "2024-04-21T04:00:00+03:00", "2024-04-21T04:00:00+03:00")]
    public void GetLastDayOfWeek_Dates_ShouldBe(DayOfWeek dayOfWeek, string date, string expected)
    {
        var actual = DateTimeOffset.Parse(date).GetLastDayOfWeek(dayOfWeek);
        
        Assert.Equal(DateTimeOffset.Parse(expected), actual);
    }
}