using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class ResourcesManager : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly IMessageProvider _messageProvider;
    private readonly PersistentComponentState _applicationState;
    
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

    public async Task Load(CancellationToken token = default)
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

    public string this[string name]
    {
        get
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            
            var languageContext = _renderContext.GetLanguageContext();
            var resources = _resources.TryGetValue(languageContext.CurrentLanguage.Value, out var result)
                ? result
                : new Dictionary<string, string>();
            
            return resources.GetValueOrDefault(name, name);
        }
    }

    public void Dispose() => _persistingSubscription?.Dispose();
}