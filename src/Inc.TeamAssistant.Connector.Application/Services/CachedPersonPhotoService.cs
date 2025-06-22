using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Caching.Hybrid;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CachedPersonPhotoService : IPersonPhotoService
{
    private readonly IPersonPhotoService _service;
    private readonly HybridCache _cache;
    private readonly int _cacheDurationInSeconds;

    public CachedPersonPhotoService(IPersonPhotoService service, HybridCache cache, int cacheDurationInSeconds)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheDurationInSeconds = cacheDurationInSeconds;
    }

    public async Task<byte[]> GetPersonPhoto(long personId, CancellationToken token)
    {
        return await _cache.GetOrCreateAsync(
            $"{nameof(CachedPersonPhotoService)}__{nameof(GetPersonPhoto)}__{personId}",
            personId,
            async (pId, t) => await _service.GetPersonPhoto(pId, t),
            CreateCacheOptions(),
            cancellationToken: token);
    }

    private HybridCacheEntryOptions CreateCacheOptions()
    {
        const int maxCacheDurationRandomComponentInSeconds = 60 * 5;
        
        var cacheDurationRandomComponentInSeconds = Random.Shared.Next(1, maxCacheDurationRandomComponentInSeconds);
            
        return new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromSeconds(_cacheDurationInSeconds + cacheDurationRandomComponentInSeconds)
        };
    }
}