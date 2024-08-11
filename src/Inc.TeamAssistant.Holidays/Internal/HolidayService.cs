namespace Inc.TeamAssistant.Holidays.Internal;

internal sealed class HolidayService : IHolidayService
{
    private readonly IHolidayReader _reader;

    public HolidayService(IHolidayReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<bool> IsWorkTime(Guid botId, DateTimeOffset value, CancellationToken token)
    {
        var calendar = await _reader.Find(botId, token);
        if (calendar is null)
            throw new ApplicationException($"Calendar for bot {botId} not found.");

        return calendar.IsWorkTime(value.DateTime);
    }

    public async Task<TimeSpan> CalculateWorkTime(
        Guid botId,
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken token)
    {
        var calendar = await _reader.Find(botId, token);
        if (calendar is null)
            throw new ApplicationException($"Calendar for bot {botId} not found.");
        
        var interval = TimeSpan.Zero;
        var currentDate = new DateOnly(start.Year, start.Month, start.Day);
        var endDate = new DateOnly(end.Year, end.Month, end.Day);

        while (currentDate <= endDate)
        {
            var workTime = calendar.GetWorkTime(currentDate);
            
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
}