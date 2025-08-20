using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class ServerRenderContext(IHttpContextAccessor httpContextAccessor) : IRenderContext
{
    public bool IsClientSide => false;

    public LanguageId? SelectedLanguage => LanguageSettings.LanguageIds
        .SingleOrDefault(l => (httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty).StartsWith(
            $"/{l.Value}",
            StringComparison.InvariantCultureIgnoreCase));
}