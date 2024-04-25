using Blazored.LocalStorage;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.WebUI.Services.Render;

internal sealed class MessageProviderClientCached : IMessageProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IMessageProvider _messageProvider;
    private readonly string _appVersion;

    public MessageProviderClientCached(
        ILocalStorageService localStorage,
        IMessageProvider messageProvider,
        string appVersion)
    {
        if (string.IsNullOrWhiteSpace(appVersion))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(appVersion));

        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _appVersion = appVersion;
    }

    public async Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> Get()
    {
        var key = GetKey();

        if (!await _localStorage.ContainKeyAsync(key))
        {
            await _localStorage.ClearAsync();
            var response = await _messageProvider.Get();

            if (response.State == ServiceResultState.Success)
                await _localStorage.SetItemAsync(key, response.Result);
            else
                throw new TeamAssistantException("Can't load resources.");
        }

        var data = await _localStorage.GetItemAsync<Dictionary<string, Dictionary<string, string>>>(key);
        return ServiceResult.Success(data!);
    }

    private string GetKey() => $"{nameof(MessageProviderClientCached)}_{nameof(Get)}_{_appVersion}";
}