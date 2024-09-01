using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class ResourcesManager : IDisposable
{
    private readonly IRenderContext _renderContext;
    private readonly IMessageProvider _messageProvider;
    private readonly PersistentComponentState _applicationState;
    private static readonly string Key = nameof(ResourcesManager);
    
    private PersistingComponentStateSubscription? _persistingSubscription;
    private static Dictionary<string, Dictionary<string, string>> _resources = new();

    public ResourcesManager(
        IMessageProvider messageProvider,
        IRenderContext renderContext,
        PersistentComponentState applicationState)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Initialize();
    }

    private void Initialize()
    {
        _persistingSubscription ??= _applicationState.RegisterOnPersisting(Persist);
        
        if (_resources.Any())
            return;
        if (_applicationState.TryTakeFromJson<Dictionary<string, Dictionary<string, string>>>(Key, out var restored) && restored is not null)
            _resources = restored;
        else
            Task.Run(async () => _resources = await _messageProvider.Get());
    }

    public string this[string name]
    {
        get
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            
            var currentLanguage = _renderContext.GetCurrentLanguageId();
            var resources = _resources.TryGetValue(currentLanguage.Language.Value, out var result)
                ? result
                : new Dictionary<string, string>();
            
            return resources.GetValueOrDefault(name, name);
        }
    }
    
    private async Task Persist()
    {
        var person = await _messageProvider.Get();
        _applicationState.PersistAsJson(Key, person);
    }

    public void Dispose() => _persistingSubscription?.Dispose();
}