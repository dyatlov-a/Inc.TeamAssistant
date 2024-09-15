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

    public async Task<LanguageId> Get(Guid botId, long personId, CancellationToken token)
    {
        return await _memoryCache.GetOrCreateAsync(GetKey(botId, personId), async c =>
        {
            c.AbsoluteExpirationRelativeToNow = _cacheTimeout;

            return await _clientLanguageRepository.Get(botId, personId, token);
        }) ?? await _clientLanguageRepository.Get(botId, personId, token);
    }

    public async Task Upsert(Guid botId, long personId, string languageId, DateTimeOffset now, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(languageId);
        
        var key = GetKey(botId, personId);

        if (_memoryCache.TryGetValue(key, out LanguageId? cachedLanguageId) && cachedLanguageId?.Value == languageId)
            return;

        await _clientLanguageRepository.Upsert(botId, personId, languageId, now, token);
        _memoryCache.Remove(key);
    }

    private string GetKey(Guid botId, long personId) => $"{nameof(CachedClientLanguageRepository)}_{botId}_{personId}";
}