using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class QuickResponseCodeGeneratorCached : IQuickResponseCodeGenerator
{
    private readonly IMemoryCache _memoryCache;
    private readonly IQuickResponseCodeGenerator _generator;
    private readonly TimeSpan _cacheAbsoluteExpiration;

    public QuickResponseCodeGeneratorCached(
        IMemoryCache memoryCache,
        IQuickResponseCodeGenerator generator,
        TimeSpan cacheAbsoluteExpiration)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _cacheAbsoluteExpiration = cacheAbsoluteExpiration;
    }

    public string Generate(string data, string foreground, string background)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);

        var key = $"data={data}&foreground={foreground}&background{background}";
        var cacheItem = _memoryCache.GetOrCreate(
            key,
            c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);

                return _generator.Generate(data, foreground, background);
            });
        
        return cacheItem ?? _generator.Generate(key, foreground, background);
    }
}