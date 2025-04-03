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

    public Dictionary<string, Dictionary<string, string>> Get()
    {
        var cacheKey = $"{nameof(MessageProviderCached)}_{nameof(Get)}";
        var cacheItem = _memoryCache.GetOrCreate(
            cacheKey,
            c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);
                return _messageProvider.Get();
            });

        return cacheItem ?? _messageProvider.Get();
    }
}