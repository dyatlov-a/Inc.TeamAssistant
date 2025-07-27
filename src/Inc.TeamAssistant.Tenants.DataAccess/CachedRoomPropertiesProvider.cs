using Inc.TeamAssistant.Primitives.Features.Tenants;
using Microsoft.Extensions.Caching.Hybrid;

namespace Inc.TeamAssistant.Tenants.DataAccess;

internal sealed class CachedRoomPropertiesProvider : IRoomPropertiesProvider
{
    private readonly HybridCache _cache;
    private readonly IRoomPropertiesProvider _provider;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public CachedRoomPropertiesProvider(HybridCache cache, IRoomPropertiesProvider provider, TimeSpan cacheTimeout)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = cacheTimeout
        };
    }

    public async Task<RoomProperties> Get(Guid roomId, CancellationToken token)
    {
        return await _cache.GetOrCreateAsync(
            GetKey(roomId),
            roomId,
            async (k, t) => await _provider.Get(k, t),
            _cacheOptions,
            cancellationToken: token);
    }

    public async Task Set(Guid roomId, RoomProperties properties, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(properties);

        await _provider.Set(roomId, properties, token);
        
        await _cache.SetAsync(GetKey(roomId), _cacheOptions, cancellationToken: token);
    }
    
    private string GetKey(Guid roomId) => $"{nameof(CachedRoomPropertiesProvider)}_{roomId}";
}