using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class MessageProviderCached : IMessageProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMessageProvider _messageProvider;
    private readonly TimeSpan _cacheAbsoluteExpiration;

    public MessageProviderCached(
        IMemoryCache memoryCache,
        IMessageProvider messageProvider,
        TimeSpan cacheAbsoluteExpiration)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _cacheAbsoluteExpiration = cacheAbsoluteExpiration;
    }

    public async Task<Dictionary<string, Dictionary<string, string>>> Get(CancellationToken token)
    {
        var cacheKey = $"{nameof(MessageProviderCached)}_{nameof(Get)}";
        var cacheItem = await _memoryCache.GetOrCreateAsync(
            cacheKey,
            async c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);
                return await _messageProvider.Get(token);
            });

        return cacheItem ?? await _messageProvider.Get(token);
    }
}