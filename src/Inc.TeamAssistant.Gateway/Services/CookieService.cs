using Inc.TeamAssistant.Appraiser.Model;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public bool IsServerRender => true;
    
    public Task<string?> GetValue(string name)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is { } && httpContext.Request.Cookies.TryGetValue(name, out var value))
            return Task.FromResult<string?>(value);

        return Task.FromResult(default(string?));
    }

    public Task SetValue(string name, string value, int lifetimeInDays)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is { })
        {
            httpContext.Response.Cookies.Append(name, value, new()
            {
                Domain = httpContext.Request.Host.Value,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(lifetimeInDays)
            });
        }

        return Task.CompletedTask;
    }
}