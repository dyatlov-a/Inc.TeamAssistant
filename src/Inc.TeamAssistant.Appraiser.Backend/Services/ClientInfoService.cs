using Inc.TeamAssistant.Appraiser.Dashboard;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

internal sealed class ClientInfoService : IClientInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientInfoService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<LanguageId> GetCurrentLanguageId()
        => GetLanguageIdFromUrlOrDefault() ?? await GetLanguageIdFromClientOrDefault() ?? Settings.DefaultLanguageId;

    public LanguageId? GetLanguageIdFromUrlOrDefault()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
            return null;

        return httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.Length > Settings.LanguageCodeLength
            ? Settings.LanguageIds.SingleOrDefault(l => httpContext.Request.Path.StartsWithSegments($"/{l.Value}", StringComparison.InvariantCultureIgnoreCase))
            : default;
    }

    private Task<LanguageId?> GetLanguageIdFromClientOrDefault()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
            return Task.FromResult<LanguageId?>(default);

        var clientLanguage = httpContext.Request.GetTypedHeaders().AcceptLanguage.MaxBy(x => x.Quality ?? 1)?.ToString();

        var result = !string.IsNullOrWhiteSpace(clientLanguage) && clientLanguage.Length >= Settings.LanguageCodeLength
            ? Settings.LanguageIds.SingleOrDefault(l => clientLanguage.StartsWith(l.Value, StringComparison.InvariantCultureIgnoreCase))
            : null;

        return Task.FromResult(result);
    }
}