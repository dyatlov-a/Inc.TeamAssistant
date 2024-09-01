using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class ResourcesLoader : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ResourcesLoader(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task StartAsync(CancellationToken token)
    {
        using var scope = _serviceProvider.CreateScope();

        using var resourcesManager = scope.ServiceProvider.GetRequiredService<ResourcesManager>();

        await resourcesManager.Initialize(token);
    }

    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
}