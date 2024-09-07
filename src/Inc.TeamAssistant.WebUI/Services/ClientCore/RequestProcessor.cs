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
    private readonly NotificationsService? _notificationsService;
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
        _notificationsService = serviceProvider.GetService<NotificationsService>();
    }

    public async Task<LoadingState> Process<TResponse>(
        Func<Task<TResponse>> request,
        string key,
        Action<TResponse> onLoaded)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(onLoaded);
        
        if (_renderContext.IsBrowser)
        {
            if (_applicationState.TryTakeFromJson<TResponse>(key, out var restored) && restored is not null)
            {
                onLoaded(restored);
                return LoadingState.Done();
            }

            await BackgroundEnding(request, onLoaded);
            
            return LoadingState.Loading();
        }

        await ForegroundEnding(request, key, onLoaded);
        
        return LoadingState.Done();
    }
    
    public async Task<LoadingState> Process(Func<Task> request, Action onLoaded)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(onLoaded);

        await BackgroundEnding(async () =>
        {
            await request();
            return true;
        }, _ =>
        {
            onLoaded();
        });
            
        return LoadingState.Loading();
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

                await (_requestCount > 1 ? handler : Task.WhenAll(handler, Task.Delay(GlobalSettings.MinLoadingDelay)));
                
                onLoaded(handler.Result);
            }
            catch (Exception ex)
            {
                _notificationsService?.Add(Notification.Error(ex.Message));
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

        try
        {
            var response = await request();
            
            _persistingSubscriptions.Add(_applicationState.RegisterOnPersisting(() =>
            {
                _applicationState.PersistAsJson(key, response);
                return Task.CompletedTask;
            }));
        
            onLoaded(response);
        }
        catch (Exception ex)
        {
            _notificationsService?.Add(Notification.Error(ex.Message));
        }
    }

    public void Dispose()
    {
        foreach (var persistingSubscription in _persistingSubscriptions)
            persistingSubscription.Dispose();
    }
}