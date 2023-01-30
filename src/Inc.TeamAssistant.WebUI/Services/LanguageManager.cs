using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class LanguageManager
{
    private readonly IMessageService _messageService;
    private readonly IClientInfoService _clientInfoService;

    public LanguageManager(IMessageService messageService, IClientInfoService clientInfoService)
    {
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _clientInfoService = clientInfoService ?? throw new ArgumentNullException(nameof(clientInfoService));
    }

    public async Task<Dictionary<string, string>> GetResource()
    {
        var currentLanguage = await _clientInfoService.GetCurrentLanguageId();
        var resources = await _messageService.GetAll();

        return resources.Result.TryGetValue(currentLanguage.Value, out var result)
            ? result
            : resources.Result[Settings.DefaultLanguageId.Value];
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