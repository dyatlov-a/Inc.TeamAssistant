using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class ResourcesManager : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly IMessageProvider _messageProvider;
    private readonly PersistentComponentState _applicationState;
    
    private readonly SemaphoreSlim _sync = new(1, 1);
    private PersistingComponentStateSubscription? _persistingSubscription;
    private Dictionary<string, Dictionary<string, string>> _resources = new();

    public ResourcesManager(
        IMessageProvider messageProvider,
        IRenderContext renderContext,
        PersistentComponentState applicationState)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
    }
    
    public async Task Initialize(CancellationToken token = default)
    {
        if (_resources.Any())
            return;
        
        await _sync.WaitAsync(token);
        
        try
        {
            if (!_resources.Any())
                await Load(token);
        }
        finally
        {
            _sync.Release();
        }
    }

    private async Task Load(CancellationToken token)
    {
        const string key = nameof(ResourcesManager);
        
        var resources = _applicationState.TryTakeFromJson<Dictionary<string, Dictionary<string, string>>>(key, out var restored) && restored is not null
            ? restored
            : await _messageProvider.Get(token);
        
        _persistingSubscription ??= _applicationState.RegisterOnPersisting(() =>
        {
            _applicationState.PersistAsJson(key, resources);
            return Task.CompletedTask;
        });
        
        _resources = resources;
    }

    public string this[MessageId messageId]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(messageId);
            
            var languageContext = _renderContext.GetLanguageContext();
            var resources = _resources.TryGetValue(languageContext.CurrentLanguage.Value, out var result)
                ? result
                : new Dictionary<string, string>();
            
            return resources.GetValueOrDefault(messageId.Value, messageId.Value);
        }
    }

    public void Dispose() => _persistingSubscription?.Dispose();
}