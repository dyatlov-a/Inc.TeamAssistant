using Inc.TeamAssistant.Primitives.FeatureProperties;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModel
{
    public Guid? CalendarId { get; set; }

    private readonly List<string> _supportedLanguages = new();
    public IReadOnlyCollection<string> SupportedLanguages => _supportedLanguages;

    private readonly Dictionary<string, string> _properties = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, string> Properties => _properties;
    
    private readonly Dictionary<string, IReadOnlyCollection<SettingSection>> _availableProperties = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties => _availableProperties;

    public SetSettingsFormModel ChangeProperty(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        _properties[key] = value;
        
        return this;
    }
    
    public SetSettingsFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        CalendarId = stagesState.CalendarId;
        
        _supportedLanguages.Clear();
        _supportedLanguages.AddRange(stagesState.SupportedLanguages);
        
        _properties.Clear();
        foreach (var property in stagesState.SelectedFeatures.SelectMany(f => f.Properties))
            _properties.Add(property, stagesState.Properties.GetValueOrDefault(property, string.Empty));
        
        _availableProperties.Clear();
        foreach (var availableProperty in stagesState.AvailableProperties)
            _availableProperties.Add(availableProperty.Key, availableProperty.Value);

        return this;
    }
    
    public void SetLanguages(IEnumerable<string> languageIds)
    {
        _supportedLanguages.Clear();
        _supportedLanguages.AddRange(languageIds);
    }
}