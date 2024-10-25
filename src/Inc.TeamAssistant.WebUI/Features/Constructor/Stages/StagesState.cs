using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using Inc.TeamAssistant.Constructor.Model.Queries.GetProperties;
using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage2;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class StagesState
{
    public Guid? Id { get; private set; }
    public Guid? CalendarId { get; private set; }
    public string UserName { get; private set; }
    public string Token { get; private set; }

    private readonly List<string> _supportedLanguages = new();
    public IReadOnlyCollection<string> SupportedLanguages => _supportedLanguages;

    private readonly List<Guid> _featureIds = new();
    public IReadOnlyCollection<Guid> FeatureIds => _featureIds;
    
    private Dictionary<string, string> _properties = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, string> Properties => _properties;

    private readonly List<FeatureDto> _availableFeatures = new();
    public IReadOnlyCollection<FeatureDto> AvailableFeatures => _availableFeatures;
    
    private readonly Dictionary<string, IReadOnlyCollection<SettingSection>> _availableProperties = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties => _availableProperties;

    public IReadOnlyCollection<FeatureDto> SelectedFeatures => AvailableFeatures
        .Where(f => FeatureIds.Contains(f.Id))
        .ToArray();

    public static StagesState Empty { get; } = new(
        null,
        null,
        string.Empty,
        string.Empty,
        Array.Empty<string>(),
        Array.Empty<Guid>(),
        new Dictionary<string, string>(),
        Array.Empty<FeatureDto>(),
        new Dictionary<string, IReadOnlyCollection<SettingSection>>());

    public StagesState(
        Guid? id,
        Guid? calendarId,
        string userName,
        string token,
        IReadOnlyCollection<string> supportedLanguages,
        IReadOnlyCollection<Guid> featureIds,
        IReadOnlyDictionary<string, string> properties,
        IReadOnlyCollection<FeatureDto> availableFeatures,
        IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> availableProperties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(supportedLanguages);
        ArgumentNullException.ThrowIfNull(featureIds);
        ArgumentNullException.ThrowIfNull(availableFeatures);
        ArgumentNullException.ThrowIfNull(availableProperties);
        
        Id = id;
        CalendarId = calendarId;
        UserName = userName;
        Token = token;
        
        _supportedLanguages.Clear();
        _supportedLanguages.AddRange(supportedLanguages);
        
        _featureIds.Clear();
        _featureIds.AddRange(featureIds);
        
        _properties.Clear();
        foreach (var property in BotPropertiesBuilder.Build(properties))
            _properties.Add(property.Key, property.Value);
        
        _availableFeatures.Clear();
        _availableFeatures.AddRange(availableFeatures);
        
        _availableProperties.Clear();
        foreach (var availableProperty in availableProperties)
            _availableProperties.Add(availableProperty.Key, availableProperty.Value);
    }
    
    public static StagesState Create(
        GetBotResult bot,
        GetFeaturesResult availableFeatures,
        GetPropertiesResult availableProperties)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(availableFeatures);
        ArgumentNullException.ThrowIfNull(availableProperties);

        return new StagesState(
            bot.Id,
            bot.CalendarId,
            bot.UserName,
            bot.Token,
            bot.SupportedLanguages,
            bot.FeatureIds,
            bot.Properties,
            availableFeatures.Features,
            availableProperties.Properties);
    }
    
    public static StagesState Create(
        GetFeaturesResult availableFeatures,
        GetPropertiesResult availableProperties,
        GetCalendarByOwnerResult? calendar)
    {
        ArgumentNullException.ThrowIfNull(availableFeatures);
        ArgumentNullException.ThrowIfNull(availableProperties);

        return new StagesState(
            null,
            calendar?.Id,
            string.Empty,
            string.Empty,
            [LanguageSettings.DefaultLanguageId.Value],
            Array.Empty<Guid>(),
            new Dictionary<string, string>(),
            availableFeatures.Features,
            availableProperties.Properties);
    }

    public StagesState Apply(CheckBotFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        UserName = formModel.UserName;
        Token = formModel.Token;
        
        return this;
    }

    public StagesState Apply(SelectFeaturesFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        _featureIds.Clear();
        _featureIds.AddRange(formModel.FeatureIds);
        
        return this;
    }

    public StagesState Apply(Guid calendarId)
    {
        CalendarId = calendarId;
        
        return this;
    }

    public StagesState Apply(SettingsFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        _properties.Clear();
        foreach (var property in formModel.Properties)
            _properties.Add(property.Key, property.Value);

        _supportedLanguages.Clear();
        _supportedLanguages.AddRange(formModel.SupportedLanguages);
        
        return this;
    }
}