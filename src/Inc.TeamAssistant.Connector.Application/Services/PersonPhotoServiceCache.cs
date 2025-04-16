using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class PersonPhotoServiceCache : IPersonPhotoService
{
    private readonly IPersonPhotoService _service;
    private readonly IMemoryCache _memoryCache;
    private readonly int _cacheDurationInSeconds;

    public PersonPhotoServiceCache(IPersonPhotoService service, IMemoryCache memoryCache, int cacheDurationInSeconds)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _cacheDurationInSeconds = cacheDurationInSeconds;
    }

    public async Task<byte[]?> GetPersonPhoto(long personId, CancellationToken token)
    {
        const int maxCacheDurationRandomComponentInSeconds = 60 * 5;
        var cacheKey = $"{nameof(PersonPhotoServiceCache)}__{nameof(GetPersonPhoto)}__{personId}";
        
        return await _memoryCache.GetOrCreateAsync(cacheKey, async c =>
        {
            var cacheDurationRandomComponentInSeconds = Random.Shared.Next(1, maxCacheDurationRandomComponentInSeconds);
            var cacheDuration = TimeSpan.FromSeconds(_cacheDurationInSeconds + cacheDurationRandomComponentInSeconds);
            
            c.SetAbsoluteExpiration(cacheDuration);

            return await _service.GetPersonPhoto(personId, token);
        });
    }
}