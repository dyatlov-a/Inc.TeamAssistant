using Inc.TeamAssistant.Reviewer.All.Holidays;
using Inc.TeamAssistant.Reviewer.All.Holidays.Internal;
using Inc.TeamAssistant.Reviewer.All.Holidays.Model;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Reviewer.AllTests.Holidays.Internal;

public sealed class HolidayServiceTests
{
    private readonly HolidayService _target;
    private readonly IHolidayReader _reader;

    public HolidayServiceTests()
    {
        _reader = Substitute.For<IHolidayReader>();
        _reader.GetAll(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Dictionary<DateOnly, HolidayType>()));
        _target = new(_reader);
    }

    [Theory]
    [InlineData("2023-01-23", true)]
    [InlineData("2023-01-24", true)]
    [InlineData("2023-01-25", true)]
    [InlineData("2023-01-26", true)]
    [InlineData("2023-01-27", true)]
    [InlineData("2023-01-28", false)]
    [InlineData("2023-01-29", false)]
    public async Task IsWorkday_Date_ShouldBe(string date, bool isWorkday)
    {
        var actual = await _target.IsWorkday(DateOnly.Parse(date), CancellationToken.None);
        
        Assert.Equal(isWorkday, actual);
    }

    [Fact]
    public async Task IsWorkday_ExcludeHoliday_ShouldBeHoliday()
    {
        var day = new DateOnly(2023, 1, 23);
        _reader.GetAll(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Dictionary<DateOnly, HolidayType>
        {
            [day] = HolidayType.Holiday
        }));
        
        var actual = await _target.IsWorkday(day, CancellationToken.None);
        
        Assert.False(actual);
    }
    
    [Fact]
    public async Task IsWorkday_ExcludeWorkday_ShouldBeWorkday()
    {
        var day = new DateOnly(2023, 1, 28);
        _reader.GetAll(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Dictionary<DateOnly, HolidayType>
        {
            [day] = HolidayType.Workday
        }));
        
        var actual = await _target.IsWorkday(day, CancellationToken.None);
        
        Assert.True(actual);
    }
}