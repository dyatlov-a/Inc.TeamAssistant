using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Gateway.Services.Internal;

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

    public string Generate(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(data));
        
        var cacheItem = _memoryCache.GetOrCreate(
            data,
            c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);

                return _generator.Generate(data);
            });
        
        if (cacheItem is null)
        {
            _logger.LogWarning("Can not get object with key {CacheKey} from cache", data);
            return _generator.Generate(data);
        }

        return cacheItem;
    }
}