using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Services.Render;

internal sealed class LanguageManager
{
    private readonly IMessageProvider _messageProvider;
    private readonly ILanguageProvider _languageProvider;

    public LanguageManager(IMessageProvider messageProvider, ILanguageProvider languageProvider)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _languageProvider = languageProvider ?? throw new ArgumentNullException(nameof(languageProvider));
    }

    public async Task<Dictionary<string, string>> GetResource()
    {
        var currentLanguage = _languageProvider.GetCurrentLanguageId();
        var resources = await _messageProvider.Get();

        return resources.Result.TryGetValue(currentLanguage.Language.Value, out var result)
            ? result
            : resources.Result[LanguageSettings.DefaultLanguageId.Value];
    }

    public Func<string?, string> CreateLinkBuilder()
    {
        var currentLanguageId = _languageProvider.GetCurrentLanguageId();

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