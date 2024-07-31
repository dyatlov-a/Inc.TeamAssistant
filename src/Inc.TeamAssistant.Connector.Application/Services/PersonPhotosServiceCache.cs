using Inc.TeamAssistant.Connector.Application.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class PersonPhotosServiceCache : IPersonPhotosService
{
    private readonly IPersonPhotosService _service;
    private readonly IMemoryCache _memoryCache;
    private readonly int _cacheDurationInSeconds;

    public PersonPhotosServiceCache(IPersonPhotosService service, IMemoryCache memoryCache, int cacheDurationInSeconds)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _cacheDurationInSeconds = cacheDurationInSeconds;
    }

    public async Task<MemoryStream?> GetPersonPhoto(long personId, CancellationToken token)
    {
        const int maxCacheDurationRandomComponentInSeconds = 60 * 5;
        
        return await _memoryCache.GetOrCreateAsync($"user_avatar__{personId}", async c =>
        {
            var cacheDurationRandomComponentInSeconds = Random.Shared.Next(0, maxCacheDurationRandomComponentInSeconds);
            var cacheDuration = TimeSpan.FromSeconds(_cacheDurationInSeconds + cacheDurationRandomComponentInSeconds);
            
            c.SetAbsoluteExpiration(cacheDuration);

            return await _service.GetPersonPhoto(personId, token);
        });
    }
}