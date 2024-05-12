using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Core;

public sealed class LanguageManager
{
    private readonly IMessageProvider _messageProvider;
    private readonly IRenderContext _renderContext;

    public LanguageManager(IMessageProvider messageProvider, IRenderContext renderContext)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
    }

    public async Task<Dictionary<string, string>> GetResource()
    {
        var currentLanguage = _renderContext.GetCurrentLanguageId();
        var resources = await _messageProvider.Get();

        return resources.Result.TryGetValue(currentLanguage.Language.Value, out var result)
            ? result
            : resources.Result[LanguageSettings.DefaultLanguageId.Value];
    }

    public Func<string?, string> CreateLinkBuilder()
    {
        var currentLanguageId = _renderContext.GetCurrentLanguageId();

        return relativeUrl => CreateLinkBuilder(
            currentLanguageId.Selected ? currentLanguageId.Language : null,
            relativeUrl);
    }

    private string CreateLinkBuilder(LanguageId? currentLanguageId, string? relativeUrl)
    {
        var link = string.IsNullOrWhiteSpace(relativeUrl) ? "/" : $"/{relativeUrl}";

        return currentLanguageId is not null
            ? $"/{currentLanguageId.Value}{link}"
            : link;
    }
}