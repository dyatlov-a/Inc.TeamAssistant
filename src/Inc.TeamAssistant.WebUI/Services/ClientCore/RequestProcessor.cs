using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Components;
using Inc.TeamAssistant.WebUI.Components.Notifications;
using Inc.TeamAssistant.WebUI.Contracts;
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
    private readonly ILogger<RequestProcessor> _logger;

    public RequestProcessor(
        IRenderContext renderContext,
        PersistentComponentState applicationState,
        IServiceProvider serviceProvider,
        ILogger<RequestProcessor> logger)
    {
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationsService = serviceProvider.GetService<INotificationsService>();
    }
    
    public async Task<TResponse> Process<TResponse>(
        Func<Task<TResponse>> request,
        string key,
        IProgress<LoadingState.State> progress,
        bool showLoading = true)
        where TResponse : IWithEmpty<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(progress);
        
        if (_renderContext.IsBrowser)
        {
            if (_applicationState.TryTakeFromJson<TResponse>(key, out var restored) && restored is not null)
            {
                progress.Report(LoadingState.State.Done);
                
                return restored;
            }
            
            if (showLoading)
                progress.Report(LoadingState.State.Loading);
            
            return await BackgroundEnding(request, progress);
        }

        return await ForegroundEnding(request, key, progress);
    }
    
    public async Task<TResponse> Process<TResponse>(
        Func<Task<TResponse>> request,
        IProgress<LoadingState.State> progress,
        bool showLoading = true)
        where TResponse : IWithEmpty<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);
        
        if (showLoading)
            progress.Report(LoadingState.State.Loading);
        
        return await BackgroundEnding(request, progress);
    }
    
    public async Task Process(
        Func<Task> request,
        IProgress<LoadingState.State> progress,
        bool showLoading = true)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);
        
        if (showLoading)
            progress.Report(LoadingState.State.Loading);
        
        await BackgroundEnding(async () =>
            {
                await request();
                return IdleResult.Empty;
            }, progress);
    }
    
    private async Task<TResponse> BackgroundEnding<TResponse>(
        Func<Task<TResponse>> request,
        IProgress<LoadingState.State> progress)
        where TResponse : IWithEmpty<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);

        Interlocked.Increment(ref _requestCount);
        await _sync.WaitAsync();

        try
        {
            var handler = request();

            await (_requestCount > 1 ? handler : Task.WhenAll(handler, Task.Delay(GlobalSettings.MinLoadingDelay)));

            progress.Report(LoadingState.State.Done);

            return handler.Result;
        }
        catch (Exception ex)
        {
            _notificationsService?.Publish(Notification.Error(ex.Message));

            progress.Report(LoadingState.State.Error);
        }
        finally
        {
            _sync.Release();
            Interlocked.Decrement(ref _requestCount);
        }

        return TResponse.Empty;
    }

    private async Task<TResponse> ForegroundEnding<TResponse>(
        Func<Task<TResponse>> request,
        string key,
        IProgress<LoadingState.State> progress)
        where TResponse : IWithEmpty<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(progress);

        try
        {
            var response = await request();

            _persistingSubscriptions.Add(_applicationState.RegisterOnPersisting(() =>
            {
                _applicationState.PersistAsJson(key, response);
                return Task.CompletedTask;
            }));
            
            progress.Report(LoadingState.State.Done);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on request foreground ending");
            
            progress.Report(LoadingState.State.Error);
        }

        return TResponse.Empty;
    }
    
    private sealed class IdleResult : IWithEmpty<IdleResult>
    {
        public static IdleResult Empty { get; } = new();
    }

    public void Dispose()
    {
        foreach (var persistingSubscription in _persistingSubscriptions)
            persistingSubscription.Dispose();
    }
}