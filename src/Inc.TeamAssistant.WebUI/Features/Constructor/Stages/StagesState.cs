using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
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
    public IReadOnlyCollection<string> SupportedLanguages { get; private set; }
    public IReadOnlyCollection<Guid> FeatureIds { get; private set; }
    public IReadOnlyDictionary<string, string> Properties { get; private set; }
    
    public IReadOnlyCollection<FeatureDto> AvailableFeatures { get; private set; }
    public IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties { get; private set; }

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
        
        Id = id;
        CalendarId = calendarId;
        UserName = userName;
        Token = token;
        SupportedLanguages = supportedLanguages ?? throw new ArgumentNullException(nameof(supportedLanguages));
        FeatureIds = featureIds ?? throw new ArgumentNullException(nameof(featureIds));
        Properties = BotPropertiesBuilder.Build(properties);
        AvailableFeatures = availableFeatures ?? throw new ArgumentNullException(nameof(availableFeatures));
        AvailableProperties = availableProperties ?? throw new ArgumentNullException(nameof(availableProperties));
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
        GetPropertiesResult availableProperties)
    {
        ArgumentNullException.ThrowIfNull(availableFeatures);
        ArgumentNullException.ThrowIfNull(availableProperties);

        return new StagesState(
            null,
            null,
            string.Empty,
            string.Empty,
            new [] { LanguageSettings.DefaultLanguageId.Value },
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
        
        var selectedProperties = SelectedFeatures
            .SelectMany(f => f.Properties)
            .ToArray();
        
        FeatureIds = formModel.FeatureIds.ToArray();
        Properties = Properties
            .Where(p => selectedProperties.Contains(p.Key, StringComparer.InvariantCultureIgnoreCase))
            .ToDictionary();
        
        return this;
    }

    public StagesState Apply(Guid calendarId)
    {
        CalendarId = calendarId;
        
        return this;
    }

    public StagesState Apply(SetSettingsFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        Properties = formModel.Properties.ToDictionary(v => v.Title, v => v.Value);
        SupportedLanguages = formModel.SupportedLanguages.ToArray();
        
        return this;
    }
}