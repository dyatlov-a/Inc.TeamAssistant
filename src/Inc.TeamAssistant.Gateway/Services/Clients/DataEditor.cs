using System.Collections.Concurrent;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class DataEditor : IDataEditor
{
    private static readonly ConcurrentDictionary<(long UserId, string Key), string> Store = new();
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataEditor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Task<ServiceResult<string?>> Get(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var sharedKey = ToSharedKey(key);
        var data = Store.GetValueOrDefault(sharedKey);
        
        return Task.FromResult(ServiceResult.Success(data));
    }

    public Task Attach(string key, string data, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        
        var sharedKey = ToSharedKey(key);
        Store.AddOrUpdate(sharedKey, data, (_, _) => data);
        
        return Task.CompletedTask;
    }

    public Task Detach(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var sharedKey = ToSharedKey(key);
        Store.TryRemove(sharedKey, out _);
        
        return Task.CompletedTask;
    }

    private (long UserId, string Key) ToSharedKey(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var currentPerson = _httpContextAccessor.HttpContext!.User.ToPerson();
        
        return (currentPerson.Id, key);
    }
}