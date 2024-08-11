using Inc.TeamAssistant.Holidays.Model;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Holidays.Internal;

internal sealed class HolidayReaderCache : IHolidayReader
{
    private readonly IHolidayReader _reader;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheTimeout;

    public HolidayReaderCache(
        IHolidayReader reader,
        IMemoryCache memoryCache,
        TimeSpan cacheTimeout)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _cacheTimeout = cacheTimeout;
    }

    public async Task<Calendar?> Find(Guid botId, CancellationToken token)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"{nameof(HolidayReaderCache)}__{nameof(Find)}__{botId}",
            e =>
            {
                e.SetAbsoluteExpiration(_cacheTimeout);

                return _reader.Find(botId, token);
            });
    }
}