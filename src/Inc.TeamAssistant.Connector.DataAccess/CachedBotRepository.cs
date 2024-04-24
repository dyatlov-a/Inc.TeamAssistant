using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class CachedBotRepository : IBotRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IBotRepository _botRepository;
    private readonly TimeSpan _cacheTimeout;

    public CachedBotRepository(
        IMemoryCache memoryCache,
        IBotRepository botRepository,
        TimeSpan cacheTimeout)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _cacheTimeout = cacheTimeout;
    }
    
    public async Task<IReadOnlyCollection<Guid>> GetBotIds(CancellationToken token)
    {
        return await _botRepository.GetBotIds(token);
    }

    public async Task<string> GetBotName(Guid id, CancellationToken token)
    {
        return await _memoryCache.GetOrCreateAsync(GetKey(id), async c =>
        {
            c.AbsoluteExpirationRelativeToNow = _cacheTimeout;

            return await _botRepository.GetBotName(id, token);
        }) ?? await _botRepository.GetBotName(id, token);
    }

    public async Task<Bot?> Find(Guid id, CancellationToken token)
    {
        return await _botRepository.Find(id, token);
    }
    
    private string GetKey(Guid botId) => $"{nameof(CachedBotRepository)}_{botId}";
}