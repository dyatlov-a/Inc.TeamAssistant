using Blazored.LocalStorage;
using Inc.TeamAssistant.WebUI.Contracts;

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

    public async Task<Dictionary<string, Dictionary<string, string>>> Get(CancellationToken token)
    {
        var key = GetKey();

        if (!await _localStorage.ContainKeyAsync(key, token))
        {
            await _localStorage.ClearAsync(token);
            
            var response = await _messageProvider.Get(token);

            await _localStorage.SetItemAsync(key, response, token);
        }

        var data = await _localStorage.GetItemAsync<Dictionary<string, Dictionary<string, string>>>(key, token);
        return data!;
    }

    private string GetKey() => $"{nameof(MessageProviderClientCached)}_{nameof(Get)}_{_appVersion}";
}