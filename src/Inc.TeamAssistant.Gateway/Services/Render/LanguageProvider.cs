using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Gateway.Services.Render;

internal sealed class LanguageProvider : ILanguageProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LanguageProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

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