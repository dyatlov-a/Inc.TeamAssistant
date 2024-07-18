using System.Collections.Concurrent;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class DataEditor : IDataEditor
{
    private readonly ConcurrentDictionary<Guid, string> _store = new();
    
    public Task<ServiceResult<string?>> Get(Guid dataId, CancellationToken token)
    {
        var data = _store.GetValueOrDefault(dataId);
        
        return Task.FromResult(ServiceResult.Success(data));
    }

    public Task Attach(Guid dataId, string data, CancellationToken token)
    {
        _store.AddOrUpdate(dataId, data, (_, _) => data);
        
        return Task.CompletedTask;
    }
}