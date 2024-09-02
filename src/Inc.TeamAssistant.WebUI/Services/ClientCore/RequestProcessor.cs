using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class RequestProcessor : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly PersistentComponentState _applicationState;
    private readonly List<PersistingComponentStateSubscription> _persistingSubscriptions = new();
    
    private readonly SemaphoreSlim _sync = new(1, 1);
    private volatile int _requestCount;

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

        await (hasLoading ? BackgroundEnding(request, onLoaded) : ForegroundEnding(request, key, onLoaded));

        return hasLoading ? RequestState.Loading() : RequestState.Done();
    }

    private Task BackgroundEnding<TResponse>(Func<Task<TResponse>> request, Action<TResponse> onLoaded)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(onLoaded);

        Task.Run(async () =>
        {
            Interlocked.Increment(ref _requestCount);
            await _sync.WaitAsync();

            try
            {
                var handler = request();

                await (_requestCount > 1 ? handler : Task.WhenAll(handler, Task.Delay(GlobalSettings.LoadingDelay)));

                onLoaded(handler.Result);
            }
            catch (Exception ex)
            {
                // TODO: show error message for the user
            }
            finally
            {
                _sync.Release();
                Interlocked.Decrement(ref _requestCount);
            }
        });
        
        return Task.CompletedTask;
    }

    private async Task ForegroundEnding<TResponse>(
        Func<Task<TResponse>> request,
        string key,
        Action<TResponse> onLoaded)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(onLoaded);
        
        var response = await request();
        
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