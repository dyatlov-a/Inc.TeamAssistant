using Inc.TeamAssistant.Primitives;
using Microsoft.Extensions.Caching.Hybrid;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class QuickResponseCodeGeneratorCached : IQuickResponseCodeGenerator
{
    private readonly HybridCache _cache;
    private readonly IQuickResponseCodeGenerator _generator;
    private readonly HybridCacheEntryOptions _cacheOptions;

    public QuickResponseCodeGeneratorCached(
        HybridCache cache,
        IQuickResponseCodeGenerator generator,
        TimeSpan cacheTimeout)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = cacheTimeout
        };
    }

    public async Task<string> Generate(string data, string foreground, string background, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);
        
        return await _cache.GetOrCreateAsync(
            $"data={data}&foreground={foreground}&background{background}",
            (Data: data, Foreground: foreground, Background: background),
            async (s, t) => await _generator.Generate(s.Data, s.Foreground, s.Background, t),
            _cacheOptions,
            cancellationToken: token);
    }
}