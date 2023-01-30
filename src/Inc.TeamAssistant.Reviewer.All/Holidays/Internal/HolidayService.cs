using Inc.TeamAssistant.Reviewer.All.Holidays.Model;

namespace Inc.TeamAssistant.Reviewer.All.Holidays.Internal;

internal sealed class HolidayService : IHolidayService
{
    private readonly IHolidayReader _reader;

    public HolidayService(IHolidayReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<bool> IsWorkday(DateOnly date, CancellationToken cancellationToken)
    {
        var holidays = await _reader.GetAll(cancellationToken);

        if (holidays.TryGetValue(date, out var holidayType))
            return holidayType == HolidayType.Workday;

        return date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
    }
}