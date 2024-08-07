using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Gateway.Services.Render;

internal sealed class MessageProviderCached : IMessageProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MessageProviderCached> _logger;
    private readonly IMessageProvider _messageProvider;
    private readonly TimeSpan _cacheAbsoluteExpiration;

    public MessageProviderCached(
        IMemoryCache memoryCache,
        ILogger<MessageProviderCached> logger,
        IMessageProvider messageProvider,
        TimeSpan cacheAbsoluteExpiration)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _cacheAbsoluteExpiration = cacheAbsoluteExpiration;
    }

    public async Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> Get(CancellationToken token)
    {
        var cacheKey = $"{nameof(MessageProviderCached)}_{nameof(Get)}";
        var cacheItem = await _memoryCache.GetOrCreateAsync(
            cacheKey,
            async c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);
                return await _messageProvider.Get(token);
            });
        
        if (cacheItem is null)
        {
            _logger.LogWarning("Can not get object with key {CacheKey} from cache", cacheKey);
            return await _messageProvider.Get(token);
        }

        return cacheItem;
    }
}