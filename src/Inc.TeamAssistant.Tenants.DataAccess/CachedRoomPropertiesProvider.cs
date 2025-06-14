using Inc.TeamAssistant.Tenants.Application.Contracts;
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

    public async Task<T> Get<T>(Guid roomId, CancellationToken token)
        where T : class, new()
    {
        return await _cache.GetOrCreateAsync(
            GetKey<T>(roomId),
            roomId,
            async (k, t) => await _provider.Get<T>(k, t),
            _cacheOptions,
            cancellationToken: token);
    }

    public async Task Set<T>(Guid roomId, T properties, CancellationToken token)
        where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(properties);

        await _provider.Set(roomId, properties, token);
        
        await _cache.SetAsync(GetKey<T>(roomId), _cacheOptions, cancellationToken: token);
    }
    
    private string GetKey<T>(Guid roomId) => $"{nameof(CachedRoomPropertiesProvider)}_{typeof(T).Name}_{roomId}";
}