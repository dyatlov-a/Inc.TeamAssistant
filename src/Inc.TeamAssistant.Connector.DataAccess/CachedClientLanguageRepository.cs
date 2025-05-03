using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Primitives.Languages;
using Microsoft.Extensions.Caching.Hybrid;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class CachedClientLanguageRepository : IClientLanguageRepository
{
    private readonly HybridCache _cache;
    private readonly IClientLanguageRepository _clientLanguageRepository;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public CachedClientLanguageRepository(
        HybridCache cache,
        IClientLanguageRepository clientLanguageRepository,
        TimeSpan cacheTimeout)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _clientLanguageRepository = clientLanguageRepository ?? throw new ArgumentNullException(nameof(clientLanguageRepository));
        _cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = cacheTimeout
        };
    }

    public async Task<LanguageId> Get(Guid botId, long personId, CancellationToken token)
    {
        return await _cache.GetOrCreateAsync(
            GetKey(botId, personId),
            (botId, personId),
            async (s, t) => await _clientLanguageRepository.Get(s.botId, s.personId, t),
            _cacheOptions,
            cancellationToken: token);
    }

    public async Task Upsert(Guid botId, long personId, string languageId, DateTimeOffset now, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(languageId);

        var cachedLanguageId = await Get(botId, personId, token);
        if (cachedLanguageId.Value == languageId)
            return;
        
        await _clientLanguageRepository.Upsert(botId, personId, languageId, now, token);

        await _cache.RemoveAsync(GetKey(botId, personId), token);
    }

    private string GetKey(Guid botId, long personId) => $"{nameof(CachedClientLanguageRepository)}_{botId}_{personId}";
}