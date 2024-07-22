using Blazored.LocalStorage;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

internal sealed class DataEditor
{
    private readonly ILocalStorageService _localStorage;
    private readonly string _appVersion;

    public DataEditor(ILocalStorageService localStorage, string appVersion)
    {
        if (string.IsNullOrWhiteSpace(appVersion))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(appVersion));

        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _appVersion = appVersion;
    }

    public async Task<T> GetValueOrDefault<T>(
        string key,
        Func<CancellationToken, Task<T>> defaultFactory,
        CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(defaultFactory);

        var storeKey = ToStoreKey(key);

        if (await _localStorage.ContainKeyAsync(storeKey, token))
        {
            var valueFromStore = await _localStorage.GetItemAsync<T>(storeKey, token);
            return valueFromStore!;
        }
        
        var value = await defaultFactory(token);
        await Attach(key, value, token);
        return value;
    }

    public async Task Attach<T>(string key, T data, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(data);
        
        await _localStorage.SetItemAsync(ToStoreKey(key), data, token);
    }

    public async Task Detach(string key, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        await _localStorage.RemoveItemAsync(ToStoreKey(key), token);
    }
    
    private string ToStoreKey(string key) => $"{_appVersion}_{key}";
}