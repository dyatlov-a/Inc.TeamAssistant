using Inc.TeamAssistant.Holidays.Model;
using Microsoft.Extensions.Caching.Hybrid;

namespace Inc.TeamAssistant.Holidays.Internal;

internal sealed class HolidayReaderCache : IHolidayReader
{
    private readonly IHolidayReader _reader;
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public HolidayReaderCache(IHolidayReader reader, HybridCache cache, TimeSpan cacheTimeout)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = cacheTimeout
        };
    }

    public async Task<Calendar?> Find(Guid botId, CancellationToken token)
    {
        return await _cache.GetOrCreateAsync(
            GetKey(botId),
            botId,
            async (bId, t) => await _reader.Find(bId, t),
            _cacheOptions,
            cancellationToken: token);
    }

    public async Task Reload(Guid botId, CancellationToken token)
    {
        await _cache.RemoveAsync(GetKey(botId), cancellationToken: token);
        
        await _reader.Reload(botId, token);

        await Find(botId, token);
    }

    private string GetKey(Guid botId) => $"{nameof(HolidayReaderCache)}__{botId}";
}