using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Features.Components;
using Inc.TeamAssistant.WebUI.Features.Notifications;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class RequestProcessor : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly PersistentComponentState _applicationState;
    private readonly INotificationsService? _notificationsService;
    private readonly List<PersistingComponentStateSubscription> _persistingSubscriptions = new();
    
    private readonly SemaphoreSlim _sync = new(1, 1);
    private volatile int _requestCount;

    public RequestProcessor(
        IRenderContext renderContext,
        PersistentComponentState applicationState,
        IServiceProvider serviceProvider)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _notificationsService = serviceProvider.GetService<INotificationsService>();
    }

    public async Task Process<TResponse>(
        Func<Task<TResponse>> request,
        string key,
        Action<TResponse> onLoaded,
        Action<LoadingState> stateChanged)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(onLoaded);
        ArgumentNullException.ThrowIfNull(stateChanged);
        
        if (_renderContext.IsBrowser)
        {
            if (_applicationState.TryTakeFromJson<TResponse>(key, out var restored) && restored is not null)
            {
                onLoaded(restored);
                stateChanged(LoadingState.Done());
                return;
            }
            
            stateChanged(LoadingState.Loading());

            await BackgroundEnding(request, onLoaded, stateChanged);
            return;
        }

        await ForegroundEnding(request, key, onLoaded, stateChanged);
    }
    
    public async Task Process(
        Func<Task> request,
        Action onLoaded,
        Action<LoadingState> stateChanged)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(onLoaded);
        ArgumentNullException.ThrowIfNull(stateChanged);

        stateChanged(LoadingState.Loading());
        
        await BackgroundEnding(async () =>
        {
            await request();
            return true;
        }, _ =>
        {
            onLoaded();
        },
        stateChanged);
    }

    private Task BackgroundEnding<TResponse>(
        Func<Task<TResponse>> request,
        Action<TResponse> onLoaded,
        Action<LoadingState> stateChanged)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(onLoaded);
        ArgumentNullException.ThrowIfNull(stateChanged);

        Task.Run(async () =>
        {
            Interlocked.Increment(ref _requestCount);
            await _sync.WaitAsync();

            try
            {
                var handler = request();

                await (_requestCount > 1 ? handler : Task.WhenAll(handler, Task.Delay(GlobalSettings.MinLoadingDelay)));
                
                onLoaded(handler.Result);
                
                stateChanged(LoadingState.Done());
            }
            catch (Exception ex)
            {
                _notificationsService?.Publish(Notification.Error(ex.Message));
                
                stateChanged(LoadingState.Error());
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
        Action<TResponse> onLoaded,
        Action<LoadingState> stateChanged)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(onLoaded);

        try
        {
            var response = await request();
            
            _persistingSubscriptions.Add(_applicationState.RegisterOnPersisting(() =>
            {
                _applicationState.PersistAsJson(key, response);
                return Task.CompletedTask;
            }));
        
            onLoaded(response);
            
            stateChanged(LoadingState.Done());
        }
        catch
        {
            stateChanged(LoadingState.Error());
        }
    }

    public void Dispose()
    {
        foreach (var persistingSubscription in _persistingSubscriptions)
            persistingSubscription.Dispose();
    }
}