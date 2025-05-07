using AutoFixture;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Holidays.Internal;
using Inc.TeamAssistant.Holidays.Model;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.HolidaysTests.Internal;

public sealed class HolidayReaderCacheTests
{
    private readonly Fixture _fixture = new();
    private readonly Calendar _calendar;
    private readonly HolidayReaderCache _target;

    public HolidayReaderCacheTests()
    {
        var calendar = Calendar.Create(_fixture.Create<Guid>(), _fixture.Create<long>())
            .SetSchedule(new WorkScheduleUtc(
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(10)),
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(19))))
            .AddWeekend(DayOfWeek.Saturday)
            .AddWeekend(DayOfWeek.Sunday)
            .AddHoliday(DateOnly.FromDateTime(_fixture.Create<DateTime>()), HolidayType.Holiday);

        _calendar = calendar;
        _target = Build(calendar);
    }
    
    [Fact]
    public async Task Find_Calendar_CorrectDeserialize()
    {
        await _target.Find(_calendar.Id, CancellationToken.None);

        var actual = await _target.Find(_calendar.Id, CancellationToken.None);

        Assert.NotNull(actual);
        Assert.Equal(_calendar.Id, actual.Id);
        Assert.Equal(_calendar.OwnerId, actual.OwnerId);
        Assert.Equal(_calendar.Schedule, actual.Schedule);
        Assert.NotNull(actual.Schedule);
        Assert.Equal(_calendar.Weekends, actual.Weekends);
        Assert.NotEmpty(actual.Weekends);
        Assert.Equal(_calendar.Holidays, actual.Holidays);
        Assert.NotEmpty(actual.Holidays);
    }

    private HolidayReaderCache Build(Calendar calendar)
    {
        ArgumentNullException.ThrowIfNull(calendar);
        
        var services = new ServiceCollection();
        var holidayReader = Substitute.For<IHolidayReader>();
        
        holidayReader
            .Find(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(calendar);
        services
            .AddSingleton(holidayReader)
            .AddSingleton(sp => ActivatorUtilities.CreateInstance<HolidayReaderCache>(sp, TimeSpan.FromMinutes(10)))
            .AddHybridCache();
        
        var serviceProvider = services.BuildServiceProvider();
        var cacheService = serviceProvider.GetRequiredService<HolidayReaderCache>();

        return cacheService;
    }
}