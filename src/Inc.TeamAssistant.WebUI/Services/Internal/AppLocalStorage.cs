using Blazored.LocalStorage;

namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class AppLocalStorage
{
    private readonly ILocalStorageService _localStorage;
    private readonly string _appVersion;

    public AppLocalStorage(ILocalStorageService localStorage, string appVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appVersion);

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
        
        var storeKey = ToStoreKey<T>(key);
        
        await EnsureStoreVersion(token);

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
        
        await _localStorage.SetItemAsync(ToStoreKey<T>(key), data, token);
    }

    public async Task Detach<T>(string key, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        await _localStorage.RemoveItemAsync(ToStoreKey<T>(key), token);
    }

    private async Task EnsureStoreVersion(CancellationToken token)
    {
        foreach (var key in await _localStorage.KeysAsync(token))
            if (!key.StartsWith(_appVersion))
                await _localStorage.RemoveItemAsync(key, token);
    }
    
    private string ToStoreKey<T>(string key) => $"{_appVersion}_{typeof(T).Name}_{key}";
}