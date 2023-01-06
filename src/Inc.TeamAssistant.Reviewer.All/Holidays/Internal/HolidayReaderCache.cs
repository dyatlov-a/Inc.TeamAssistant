using Inc.TeamAssistant.Reviewer.All.Holidays.Model;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Reviewer.All.Holidays.Internal;

internal sealed class HolidayReaderCache : IHolidayReader
{
    private readonly IHolidayReader _reader;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheTimeout;

    public HolidayReaderCache(IHolidayReader reader, IMemoryCache memoryCache, TimeSpan cacheTimeout)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _cacheTimeout = cacheTimeout;
    }

    public Task<Dictionary<DateOnly, HolidayType>> GetAll(CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(
            $"{nameof(HolidayReaderCache)}__{nameof(GetAll)}",
            e =>
            {
                e.SetAbsoluteExpiration(_cacheTimeout);

                return _reader.GetAll(cancellationToken);
            });
    }
}