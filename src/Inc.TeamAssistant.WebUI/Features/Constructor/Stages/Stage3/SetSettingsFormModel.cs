using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Features.Components;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModel
{
    public Guid? CalendarId { get; set; }
    public string Token { get; set; } = string.Empty;
    public List<string> SupportedLanguages { get; set; } = new();
    public IReadOnlyCollection<SelectItem<string>> Properties { get; set; } = Array.Empty<SelectItem<string>>();
    public IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties { get; set; } = new Dictionary<string, IReadOnlyCollection<SettingSection>>();

    public SetSettingsFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        CalendarId = stagesState.CalendarId;
        Token = stagesState.Token;
        SupportedLanguages = stagesState.SupportedLanguages.ToList();
        Properties = stagesState.SelectedFeatures
            .SelectMany(f => f.Properties.Select(v => new SelectItem<string>(
                v,
                stagesState.Properties.GetValueOrDefault(v, string.Empty))))
            .ToArray();
        AvailableProperties = stagesState.AvailableProperties.ToDictionary();

        return this;
    }
    
    public void ToggleLanguage(LanguageId languageId)
    {
        var languageCode = languageId.Value;
        
        if (SupportedLanguages.Contains(languageCode, StringComparer.InvariantCultureIgnoreCase))
            SupportedLanguages.Remove(languageCode);
        else
            SupportedLanguages.Add(languageCode);
    }
}