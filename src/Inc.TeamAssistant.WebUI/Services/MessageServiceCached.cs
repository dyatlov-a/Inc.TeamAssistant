using Blazored.LocalStorage;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class MessageServiceCached : IMessageService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IMessageService _messageService;
    private readonly string _appVersion;

    public MessageServiceCached(
        ILocalStorageService localStorage,
        IMessageService messageService,
        string appVersion)
    {
        if (string.IsNullOrWhiteSpace(appVersion))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(appVersion));

        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _appVersion = appVersion;
    }

    public async Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> GetAll(CancellationToken cancellationToken)
    {
        var key = GetKey();

        if (!await _localStorage.ContainKeyAsync(key))
        {
            await _localStorage.ClearAsync();
            var response = await _messageService.GetAll(cancellationToken);

            if (response.State == ServiceResultState.Success)
                await _localStorage.SetItemAsync(key, response.Result);
            else
                throw new ApplicationException("Can't load resources.");
        }

        var data = await _localStorage.GetItemAsync<Dictionary<string, Dictionary<string, string>>>(key);
        return ServiceResult.Success(data);
    }

    private string GetKey() => $"{nameof(MessageServiceCached)}_{nameof(GetAll)}_{_appVersion}";
}