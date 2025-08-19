using Inc.TeamAssistant.Gateway.Services.InMemory;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class OnlinePersonClearBackgroundService : BackgroundService
{
    private readonly OnlinePersonInMemoryStore _store;
    private readonly ILogger<OnlinePersonClearBackgroundService> _logger;

    public OnlinePersonClearBackgroundService(
        OnlinePersonInMemoryStore store,
        ILogger<OnlinePersonClearBackgroundService> logger)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                _store.Clear(DateTimeOffset.UtcNow, GlobalResources.PersonStore.IdleConnectionLifetime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on clear online persons store");
            }
            
            await Task.Delay(GlobalResources.PersonStore.ClearDelay, token);
        }
    }
}