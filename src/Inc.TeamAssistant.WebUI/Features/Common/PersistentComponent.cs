using Inc.TeamAssistant.WebUI.Services.ClientCore;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Features.Common;

public abstract class PersistentComponent<TViewModel> : ComponentBase, IAsyncDisposable
    where TViewModel : IViewModel<TViewModel>
{
    [Inject]
    public PersistentComponentState ApplicationState { get; set; } = default!;
    
    [Inject]
    public ResourcesManager ResourcesManager { get; set; } = default!;
    
    private PersistingComponentStateSubscription? _persistingSubscription;
    protected TViewModel ViewModel { get; private set; } = TViewModel.Empty;
    protected Func<string?, string> LinkBuilder { get; private set; } = l => l ?? string.Empty;
    protected Func<string, string> ResourceProvider { get; private set; } = k => k;

    protected override async Task OnParametersSetAsync()
    {
        var key = TViewModel.PersistentKey;
        var resources = await ResourcesManager.GetResource();
        
        LinkBuilder = ResourcesManager.CreateLinkBuilder();
        ResourceProvider = k => resources.GetValueOrDefault(k, k);
        
        _persistingSubscription ??= ApplicationState.RegisterOnPersisting(() =>
        {
            ApplicationState.PersistAsJson(key, ViewModel);
            return Task.CompletedTask;
        });

        if (ApplicationState.TryTakeFromJson<TViewModel>(key, out var restored) && restored is not null)
            ViewModel = restored;
        else
        {
            ViewModel = await Initialize(resources);
        }
    }

    public async Task Update()
    {
        var resources = await ResourcesManager.GetResource();
        
        ViewModel = await Initialize(resources);
        
        StateHasChanged();
    }
    
    protected abstract Task<TViewModel> Initialize(Dictionary<string, string> resources);
    
    public virtual ValueTask DisposeAsync()
    {
        _persistingSubscription?.Dispose();

        return ValueTask.CompletedTask;
    }
}