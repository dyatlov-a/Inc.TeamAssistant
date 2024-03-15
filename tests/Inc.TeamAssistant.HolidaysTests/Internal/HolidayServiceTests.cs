using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Holidays.Internal;
using Inc.TeamAssistant.Holidays.Model;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.HolidaysTests.Internal;

public sealed class HolidayServiceTests
{
    private readonly HolidayService _target;
    private readonly IHolidayReader _reader;

    public HolidayServiceTests()
    {
        var options = new WorkdayOptions
        {
            WorkOnHoliday = false,
            StartTimeUtc = TimeSpan.FromHours(7),
            EndTimeUtc = TimeSpan.FromHours(16)
        };
        _reader = Substitute.For<IHolidayReader>();
        _reader.GetAll(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Dictionary<DateOnly, HolidayType>()));
        _target = new(_reader, options);
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
        var actual = await _target.IsWorkTime(DateTimeOffset.Parse(date), CancellationToken.None);
        
        Assert.Equal(isWorkday, actual);
    }

    [Fact]
    public async Task IsWorkTime_ExcludeHoliday_ShouldBeHoliday()
    {
        var value = new DateOnly(2023, 1, 23);
        _reader.GetAll(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Dictionary<DateOnly, HolidayType>
        {
            [value] = HolidayType.Holiday
        }));
        
        var actual = await _target.IsWorkTime(
            new DateTimeOffset(value.Year, value.Month, value.Day, 7, 0, 0, TimeSpan.Zero),
            CancellationToken.None);
        
        Assert.False(actual);
    }
    
    [Fact]
    public async Task IsWorkTime_ExcludeWorkday_ShouldBeWorkday()
    {
        var value = new DateOnly(2023, 1, 28);
        _reader.GetAll(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Dictionary<DateOnly, HolidayType>
        {
            [value] = HolidayType.Workday
        }));
        
        var actual = await _target.IsWorkTime(
            new DateTimeOffset(value.Year, value.Month, value.Day, 7, 0, 0, TimeSpan.Zero),
            CancellationToken.None);
        
        Assert.True(actual);
    }
}