using Inc.TeamAssistant.Holidays.Model;

namespace Inc.TeamAssistant.Holidays.Internal;

internal sealed class HolidayService : IHolidayService
{
    private readonly IHolidayReader _reader;
    private readonly WorkdayOptions _options;

    public HolidayService(IHolidayReader reader, WorkdayOptions options)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<bool> IsWorkTime(DateTimeOffset value, CancellationToken token)
    {
        if (_options.WorkOnHoliday)
            return true;

        if (value.TimeOfDay < _options.StartTimeUtc || value.TimeOfDay >= _options.EndTimeUtc)
            return false;

        return await IsWorkday(DateOnly.FromDateTime(value.DateTime), token);
    }

    public async Task<TimeSpan> CalculateWorkTime(DateTimeOffset start, DateTimeOffset end, CancellationToken token)
    {
        var interval = TimeSpan.Zero;
        var currentDate = new DateOnly(start.Year, start.Month, start.Day);
        var endDate = new DateOnly(end.Year, end.Month, end.Day);

        while (currentDate <= endDate)
        {
            var workTime = await GetWorkTime(currentDate, token);
            
            if (workTime.HasValue)
            {
                var intervalStart = workTime.Value.Start > start ? workTime.Value.Start : start;
                var intervalEnd = workTime.Value.End < end ? workTime.Value.End : end;
                var currentInterval = intervalEnd - intervalStart;
                interval += currentInterval < TimeSpan.Zero ? TimeSpan.Zero : currentInterval;
            }

            currentDate = currentDate.AddDays(1);
        }

        return interval;
    }

    private async Task<(DateTimeOffset Start, DateTimeOffset End)?> GetWorkTime(DateOnly date, CancellationToken token)
    {
        if (_options.WorkOnHoliday)
        {
            var startDefault = new DateTimeOffset(date, TimeOnly.MinValue, _options.Timezone);
            var endDefault = new DateTimeOffset(date, TimeOnly.MaxValue, _options.Timezone);

            return (startDefault, endDefault);
        }

        if (!await IsWorkday(date, token))
            return null;

        var startByCalendar = new DateTimeOffset(date, TimeOnly.FromTimeSpan(_options.StartTimeUtc), TimeSpan.Zero);
        var endByCalendar = new DateTimeOffset(date, TimeOnly.FromTimeSpan(_options.EndTimeUtc), TimeSpan.Zero);

        return (startByCalendar, endByCalendar);
    }

    private async Task<bool> IsWorkday(DateOnly date, CancellationToken token)
    {
        var holidays = await _reader.GetAll(token);

        if (holidays.TryGetValue(date, out var holidayType))
            return holidayType == HolidayType.Workday;

        return !_options.Weekends.Contains(date.DayOfWeek);
    }
}