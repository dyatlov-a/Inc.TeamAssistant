using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Languages;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class CachedClientLanguageRepository : IClientLanguageRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IClientLanguageRepository _clientLanguageRepository;
    private readonly TimeSpan _cacheTimeout;

    public CachedClientLanguageRepository(
        IMemoryCache memoryCache,
        IClientLanguageRepository clientLanguageRepository,
        TimeSpan cacheTimeout)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _clientLanguageRepository = clientLanguageRepository ?? throw new ArgumentNullException(nameof(clientLanguageRepository));
        _cacheTimeout = cacheTimeout;
    }

    public async Task<LanguageId> Get(long personId, CancellationToken token)
    {
        return await _memoryCache.GetOrCreateAsync(GetKey(personId), async c =>
        {
            c.AbsoluteExpirationRelativeToNow = _cacheTimeout;

            return await _clientLanguageRepository.Get(personId, token);
        }) ?? await _clientLanguageRepository.Get(personId, token);
    }

    public async Task Upsert(long personId, string languageId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(languageId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(languageId));
        
        var key = GetKey(personId);

        if (_memoryCache.TryGetValue(key, out LanguageId? cachedLanguageId) && cachedLanguageId?.Value == languageId)
            return;

        await _clientLanguageRepository.Upsert(personId, languageId, token);
        _memoryCache.Remove(key);
    }

    private string GetKey(long personId) => $"{nameof(CachedClientLanguageRepository)}_{personId}";
}