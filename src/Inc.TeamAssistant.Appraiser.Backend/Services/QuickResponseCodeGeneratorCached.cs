using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

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

    public string Generate(string data, int width, int height, bool drawQuietZones)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(data));

        return _memoryCache.GetOrCreate(
            GetKey(data, width, height, drawQuietZones),
            c =>
            {
                c.SetAbsoluteExpiration(_cacheAbsoluteExpiration);

                return _generator.Generate(data, width, height, drawQuietZones);
            });
    }

    private string GetKey(string data, int width, int height, bool drawQuietZones)
        => $"{data}_w{width}_h{height}_d{drawQuietZones}";
}