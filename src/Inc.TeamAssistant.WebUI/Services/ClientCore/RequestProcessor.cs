using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class RequestProcessor : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly PersistentComponentState _applicationState;
    private readonly List<PersistingComponentStateSubscription> _persistingSubscriptions = new();

    public RequestProcessor(IRenderContext renderContext, PersistentComponentState applicationState)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
    }

    public async Task<RequestState> Process<TResponse>(
        Func<Task<TResponse>> request,
        string key,
        Action<TResponse> onLoaded)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(onLoaded);

        if (_applicationState.TryTakeFromJson<TResponse>(key, out var restored) && restored is not null)
        {
            onLoaded(restored);
            return RequestState.Done();
        }
        
        return await StartLoading(request, key, onLoaded);
    }

    private async Task<RequestState> StartLoading<TResponse>(
        Func<Task<TResponse>> request,
        string key,
        Action<TResponse> onLoaded)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(onLoaded);

        var hasLoading = _renderContext.IsBrowser;

        if (hasLoading)
            Task.Run(() => EndLoading(hasLoading, request, key, onLoaded));
        else
            await EndLoading(hasLoading, request, key, onLoaded);

        return hasLoading ? RequestState.Loading() : RequestState.Done();
    }

    private async Task EndLoading<TResponse>(
        bool hasLoading,
        Func<Task<TResponse>> request,
        string key,
        Action<TResponse> onLoaded)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(onLoaded);
        
        var handler = request();

        if (hasLoading)
            await Task.WhenAll(handler, Task.Delay(3_000));
        else
            await handler;

        var response = handler.Result;
        
        _persistingSubscriptions.Add(_applicationState.RegisterOnPersisting(() =>
        {
            _applicationState.PersistAsJson(key, response);
            return Task.CompletedTask;
        }));
        
        onLoaded(response);
    }

    public void Dispose()
    {
        foreach (var persistingSubscription in _persistingSubscriptions)
            persistingSubscription.Dispose();
    }
}