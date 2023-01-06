using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Appraiser.Backend.Services.MessageProviders;

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

    public async Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> Get()
    {
        return await _memoryCache.GetOrCreateAsync(
            $"{nameof(MessageProviderCached)}_{nameof(Get)}",
            async c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);
                return await _messageProvider.Get();
            });
    }
}