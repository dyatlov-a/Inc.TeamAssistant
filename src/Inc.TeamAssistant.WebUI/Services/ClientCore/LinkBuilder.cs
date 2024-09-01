using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class LinkBuilder
{
    private readonly IRenderContext _renderContext;

    public LinkBuilder(IRenderContext renderContext)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
    }

    public string Build(string? relativeUrl)
    {
        var link = string.IsNullOrWhiteSpace(relativeUrl) ? "/" : $"/{relativeUrl}";
        var currentLanguageId = _renderContext.GetCurrentLanguageId();

        return currentLanguageId.Selected
            ? $"/{currentLanguageId.Language.Value}{link}"
            : link;
    }
}