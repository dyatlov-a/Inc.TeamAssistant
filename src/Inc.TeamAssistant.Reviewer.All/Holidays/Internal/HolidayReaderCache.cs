using Inc.TeamAssistant.Reviewer.All.Holidays.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.Reviewer.All.Holidays.Internal;

internal sealed class HolidayReaderCache : IHolidayReader
{
    private readonly IHolidayReader _reader;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<HolidayReaderCache> _logger;
    private readonly TimeSpan _cacheTimeout;

    public HolidayReaderCache(
        IHolidayReader reader,
        IMemoryCache memoryCache,
        ILogger<HolidayReaderCache> logger,
        TimeSpan cacheTimeout)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheTimeout = cacheTimeout;
    }

    public async Task<Dictionary<DateOnly, HolidayType>> GetAll(CancellationToken cancellationToken)
    {
        var cacheKey = $"{nameof(HolidayReaderCache)}__{nameof(GetAll)}";
        var cacheItem = await _memoryCache.GetOrCreateAsync(
            cacheKey,
            e =>
            {
                e.SetAbsoluteExpiration(_cacheTimeout);

                return _reader.GetAll(cancellationToken);
            });

        if (cacheItem is null)
        {
            _logger.LogWarning("Can not get object with key {CacheKey} from cache", cacheKey);
            return await _reader.GetAll(cancellationToken);
        }

        return cacheItem;
    }
}