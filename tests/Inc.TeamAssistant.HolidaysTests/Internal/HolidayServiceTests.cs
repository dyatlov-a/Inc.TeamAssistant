using AutoFixture;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Holidays.Internal;
using Inc.TeamAssistant.Holidays.Model;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.HolidaysTests.Internal;

public sealed class HolidayServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly HolidayService _target;
    private readonly Calendar _calendar;
    
    public HolidayServiceTests()
    {
        _calendar = new Calendar(_fixture.Create<Guid>(), _fixture.Create<long>())
            .SetSchedule(new WorkScheduleUtc(
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(7)),
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(16))))
            .AddWeekend(DayOfWeek.Saturday)
            .AddWeekend(DayOfWeek.Sunday);
        
        var reader = Substitute.For<IHolidayReader>();
        reader.Find(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(_calendar);
        _target = new(reader);
    }

    [Theory]
    [InlineData("2023-01-23T07:00:00+00:00", true)]
    [InlineData("2023-01-24T07:00:00+00:00", true)]
    [InlineData("2023-01-25T07:00:00+00:00", true)]
    [InlineData("2023-01-26T07:00:00+00:00", true)]
    [InlineData("2023-01-27T07:00:00+00:00", true)]
    [InlineData("2023-01-28T07:00:00+00:00", false)]
    [InlineData("2023-01-29T07:00:00+00:00", false)]
    public async Task IsWorkTime_Date_ShouldBe(string date, bool isWorkday)
    {
        var actual = await _target.IsWorkTime(
            _fixture.Create<Guid>(),
            DateTimeOffset.Parse(date),
            CancellationToken.None);
        
        Assert.Equal(isWorkday, actual);
    }

    [Fact]
    public async Task IsWorkTime_ExcludeHoliday_ShouldBeHoliday()
    {
        var value = new DateOnly(2023, 1, 23);
        _calendar.AddHoliday(value, HolidayType.Holiday);
        
        var actual = await _target.IsWorkTime(
            _fixture.Create<Guid>(),
            new DateTimeOffset(value.Year, value.Month, value.Day, 7, 0, 0, TimeSpan.Zero),
            CancellationToken.None);
        
        Assert.False(actual);
    }
    
    [Fact]
    public async Task IsWorkTime_ExcludeWorkday_ShouldBeWorkday()
    {
        var value = new DateOnly(2023, 1, 28);
        _calendar.AddHoliday(value, HolidayType.Workday);
        
        var actual = await _target.IsWorkTime(
            _fixture.Create<Guid>(),
            new DateTimeOffset(value.Year, value.Month, value.Day, 7, 0, 0, TimeSpan.Zero),
            CancellationToken.None);
        
        Assert.True(actual);
    }

    [Theory]
    [InlineData("2024-06-03T05:00:00Z", "2024-06-03T06:00:00Z", "00:00:00")]
    [InlineData("2024-06-03T06:00:00Z", "2024-06-03T08:00:00Z", "01:00:00")]
    [InlineData("2024-06-03T07:00:00Z", "2024-06-03T09:00:00Z", "02:00:00")]
    [InlineData("2024-06-03T10:00:00Z", "2024-06-03T12:00:00Z", "02:00:00")]
    [InlineData("2024-06-03T14:00:00Z", "2024-06-03T16:00:00Z", "02:00:00")]
    [InlineData("2024-06-03T15:00:00Z", "2024-06-03T17:00:00Z", "01:00:00")]
    [InlineData("2024-06-03T16:00:00Z", "2024-06-03T18:00:00Z", "00:00:00")]
    public async Task CalculateWorkTime_Intervals_ShouldBe(string start, string end, string expected)
    {
        var startValue = DateTimeOffset.Parse(start);
        var endValue = DateTimeOffset.Parse(end);

        var actual = await _target.CalculateWorkTime(
            _fixture.Create<Guid>(),
            startValue,
            endValue,
            CancellationToken.None);
        
        Assert.Equal(TimeSpan.Parse(expected), actual);
    }
    
    [Theory]
    [InlineData("2024-05-31T15:00:00Z", "2024-06-02T08:00:00Z", "01:00:00")]
    [InlineData("2024-05-31T15:00:00Z", "2024-06-03T08:00:00Z", "02:00:00")]
    [InlineData("2024-06-01T15:00:00Z", "2024-06-03T08:00:00Z", "01:00:00")]
    public async Task CalculateWorkTime_IntervalsWithHoliday_ShouldBe(string start, string end, string expected)
    {
        var startValue = DateTimeOffset.Parse(start);
        var endValue = DateTimeOffset.Parse(end);

        var actual = await _target.CalculateWorkTime(
            _fixture.Create<Guid>(),
            startValue,
            endValue,
            CancellationToken.None);
        
        Assert.Equal(TimeSpan.Parse(expected), actual);
    }
}