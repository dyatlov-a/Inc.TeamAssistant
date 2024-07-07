using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class QuickResponseCodeGeneratorCached : IQuickResponseCodeGenerator
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<QuickResponseCodeGeneratorCached> _logger;
    private readonly IQuickResponseCodeGenerator _generator;
    private readonly TimeSpan _cacheAbsoluteExpiration;

    public QuickResponseCodeGeneratorCached(
        IMemoryCache memoryCache,
        ILogger<QuickResponseCodeGeneratorCached> logger,
        IQuickResponseCodeGenerator generator,
        TimeSpan cacheAbsoluteExpiration)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _cacheAbsoluteExpiration = cacheAbsoluteExpiration;
    }

    public string Generate(string data, string foreground, string background)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(data));
        if (string.IsNullOrWhiteSpace(foreground))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(foreground));
        if (string.IsNullOrWhiteSpace(background))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(background));

        var key = $"data={data}&foreground={foreground}&background{background}";
        var cacheItem = _memoryCache.GetOrCreate(
            key,
            c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);

                return _generator.Generate(data, foreground, background);
            });

        if (cacheItem is not null)
            return cacheItem;
        
        _logger.LogWarning("Can not get object with key {CacheKey} from cache", key);
        return _generator.Generate(key, foreground, background);
    }
}