using System.Collections.Concurrent;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class DataEditor : IDataEditor
{
    private readonly ConcurrentDictionary<string, string> _store = new();
    
    public Task<ServiceResult<string?>> Get(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var data = _store.GetValueOrDefault(key);
        
        return Task.FromResult(ServiceResult.Success(data));
    }

    public Task Attach(string key, string data, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        
        _store.AddOrUpdate(key, data, (_, _) => data);
        
        return Task.CompletedTask;
    }

    public Task Detach(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        _store.TryRemove(key, out _);
        
        return Task.CompletedTask;
    }
}