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
    
    [Inject]
    public LinkBuilder LinkBuilder { get; set; } = default!;
    
    private PersistingComponentStateSubscription? _persistingSubscription;
    protected TViewModel ViewModel { get; private set; } = TViewModel.Empty;

    protected override async Task OnParametersSetAsync()
    {
        var key = TViewModel.PersistentKey;
        
        _persistingSubscription ??= ApplicationState.RegisterOnPersisting(() =>
        {
            ApplicationState.PersistAsJson(key, ViewModel);
            return Task.CompletedTask;
        });

        if (ApplicationState.TryTakeFromJson<TViewModel>(key, out var restored) && restored is not null)
            ViewModel = restored;
        else
        {
            ViewModel = await Initialize(ResourcesManager);
        }
    }

    public async Task Update()
    {
        ViewModel = await Initialize(ResourcesManager);
        
        StateHasChanged();
    }
    
    protected abstract Task<TViewModel> Initialize(ResourcesManager resources);
    
    public virtual ValueTask DisposeAsync()
    {
        _persistingSubscription?.Dispose();

        return ValueTask.CompletedTask;
    }
}