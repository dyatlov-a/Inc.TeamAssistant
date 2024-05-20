using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Render;

internal sealed class ServerRenderContext : IRenderContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public ServerRenderContext(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
    }

    public bool IsBrowser => false;

    public bool IsDevelopment() => _hostingEnvironment.IsDevelopment();

    public (LanguageId Language, bool Selected) GetCurrentLanguageId()
    {
        var relativeUrl = _httpContextAccessor.HttpContext!.Request.Path.Value;

        if (!string.IsNullOrWhiteSpace(relativeUrl))
        {
            var languageId = LanguageSettings.LanguageIds.SingleOrDefault(l => relativeUrl.StartsWith(
                $"/{l.Value}",
                StringComparison.InvariantCultureIgnoreCase));
            
            if (languageId is not null)
                return (languageId, true);
        }

        return (LanguageSettings.DefaultLanguageId, false);
    }
}