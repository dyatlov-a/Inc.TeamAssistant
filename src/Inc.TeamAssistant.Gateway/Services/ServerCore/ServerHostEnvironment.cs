using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class ServerHostEnvironment : IWebAssemblyHostEnvironment
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly NavigationManager _navigationManager;

    public ServerHostEnvironment(IWebHostEnvironment hostEnvironment, NavigationManager navigationManager)
    {
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
    }

    public string Environment => _hostEnvironment.EnvironmentName;
    public string BaseAddress => _navigationManager.BaseUri;
}