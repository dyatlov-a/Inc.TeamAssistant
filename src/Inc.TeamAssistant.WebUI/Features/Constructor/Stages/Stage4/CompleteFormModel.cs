using Inc.TeamAssistant.Primitives.FeatureProperties;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CompleteFormModel
{
    public Guid? CalendarId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    private readonly List<Guid> _featureIds = new();
    public IReadOnlyCollection<Guid> FeatureIds => _featureIds;
    
    private readonly Dictionary<string, string> _properties = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, string> Properties => _properties;

    private readonly List<string> _supportedLanguages = new();
    public IReadOnlyCollection<string> SupportedLanguages => _supportedLanguages;

    private Dictionary<string, IReadOnlyCollection<SettingSection>> _availableProperties = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties => _availableProperties;

    public CompleteFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        CalendarId = stagesState.CalendarId;
        UserName = stagesState.UserName;
        Token = stagesState.Token;

        _featureIds.Clear();
        _featureIds.AddRange(stagesState.FeatureIds);

        _properties.Clear();
        foreach (var property in stagesState.SelectedFeatures.SelectMany(f => f.Properties
                     .Select(p => (Key: p, Value: stagesState.Properties.GetValueOrDefault(p, string.Empty)))))
            _properties.Add(property.Key, property.Value);

        _supportedLanguages.Clear();
        _supportedLanguages.AddRange(stagesState.SupportedLanguages);

        _availableProperties.Clear();
        foreach (var availableProperty in stagesState.AvailableProperties)
            _availableProperties.Add(availableProperty.Key, availableProperty.Value);

        return this;
    }
}