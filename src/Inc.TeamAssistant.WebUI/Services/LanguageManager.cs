using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class LanguageManager
{
    private readonly IMessageProvider _messageProvider;
    private readonly IClientInfoService _clientInfoService;

    public LanguageManager(IMessageProvider messageProvider, IClientInfoService clientInfoService)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _clientInfoService = clientInfoService ?? throw new ArgumentNullException(nameof(clientInfoService));
    }

    public async Task<Dictionary<string, string>> GetResource()
    {
        var currentLanguage = await _clientInfoService.GetCurrentLanguageId();
        var resources = await _messageProvider.Get();

        return resources.Result.TryGetValue(currentLanguage.Value, out var result)
            ? result
            : resources.Result[LanguageSettings.DefaultLanguageId.Value];
    }

    public Func<string?, string> CreateLinkBuilder()
    {
        var currentLanguageId = _clientInfoService.GetLanguageIdFromUrlOrDefault();

        return relativeUrl => CreateLinkBuilder(currentLanguageId, relativeUrl);
    }

    private string CreateLinkBuilder(LanguageId? currentLanguageId, string? relativeUrl)
    {
        var link = string.IsNullOrWhiteSpace(relativeUrl) ? "/" : $"/{relativeUrl}";

        return currentLanguageId is not null
            ? $"/{currentLanguageId.Value}{link}"
            : link;
    }
}