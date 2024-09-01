using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class RequestProcessor : IDisposable
{
    private readonly PersistentComponentState _applicationState;
    private readonly List<PersistingComponentStateSubscription> _persistingSubscriptions = new();

    public RequestProcessor(PersistentComponentState applicationState)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
    }

    public async Task<TResponse> Process<TResponse>(Func<Task<TResponse>> request, string key)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var response = _applicationState.TryTakeFromJson<TResponse>(key, out var restored) && restored is not null
            ? restored
            : await request();
        
        _persistingSubscriptions.Add(_applicationState.RegisterOnPersisting(() =>
        {
            _applicationState.PersistAsJson(key, response);
            return Task.CompletedTask;
        }));

        return response;
    }

    public void Dispose()
    {
        foreach (var persistingSubscription in _persistingSubscriptions)
            persistingSubscription.Dispose();
    }
}