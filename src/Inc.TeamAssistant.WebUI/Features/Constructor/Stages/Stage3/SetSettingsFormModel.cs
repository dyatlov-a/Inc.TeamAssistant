using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.WebUI.Components;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModel
{
    public Guid? CalendarId { get; set; }
    public string Token { get; set; } = string.Empty;

    private readonly List<string> _supportedLanguages = new();
    public IReadOnlyCollection<string> SupportedLanguages => _supportedLanguages;

    private readonly List<SelectItem<string>> _properties = new();
    public IReadOnlyCollection<SelectItem<string>> Properties => _properties;
    
    private readonly Dictionary<string, IReadOnlyCollection<SettingSection>> _availableProperties = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties => _availableProperties;

    public SetSettingsFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        CalendarId = stagesState.CalendarId;
        Token = stagesState.Token;
        
        _supportedLanguages.Clear();
        _supportedLanguages.AddRange(stagesState.SupportedLanguages);
        
        _properties.Clear();
        _properties.AddRange(stagesState.SelectedFeatures
            .SelectMany(f => f.Properties.Select(v => new SelectItem<string>(
                v,
                stagesState.Properties.GetValueOrDefault(v, string.Empty)))));
        
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