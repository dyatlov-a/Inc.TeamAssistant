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

    private async Task<bool> IsWorkday(DateOnly date, CancellationToken token)
    {
        var holidays = await _reader.GetAll(token);

        if (holidays.TryGetValue(date, out var holidayType))
            return holidayType == HolidayType.Workday;

        return date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
    }
}