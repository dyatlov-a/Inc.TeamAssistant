using Blazored.LocalStorage;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class DataEditorClientCached : IDataEditor
{
    private readonly ILocalStorageService _localStorage;
    private readonly IDataEditor _dataEditor;

    public DataEditorClientCached(ILocalStorageService localStorage, IDataEditor dataEditor)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _dataEditor = dataEditor ?? throw new ArgumentNullException(nameof(dataEditor));
    }
    
    public async Task<ServiceResult<string?>> Get(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (await _localStorage.ContainKeyAsync(key, token))
        {
            var data = await _localStorage.GetItemAsync<string>(key, token);
            return ServiceResult.Success(data);
        }
        
        return await _dataEditor.Get(key, token);
    }

    public async Task Attach(string key, string data, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        
        await _localStorage.SetItemAsync(key, data, token);
        await _dataEditor.Attach(key, data, token);
    }

    public async Task Detach(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        await _localStorage.RemoveItemAsync(key, token);
        await _dataEditor.Detach(key, token);
    }
}