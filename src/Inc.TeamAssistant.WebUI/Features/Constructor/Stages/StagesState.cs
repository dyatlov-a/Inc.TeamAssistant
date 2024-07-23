using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage2;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class StagesState
{
    public Guid? Id { get; private set; }
    public string UserName { get; private set; }
    public string Token { get; private set; }
    public IReadOnlyCollection<Guid> FeatureIds { get; private set; }
    public IReadOnlyDictionary<string, string> Properties { get; private set; }
    public IReadOnlyCollection<FeatureDto> Features { get; private set; }
    public IReadOnlyCollection<string> SupportedLanguages { get; private set; }
    public IReadOnlyCollection<BotDetails> BotDetails { get; private set; }

    public StagesState(
        Guid? id,
        string userName,
        string token,
        IReadOnlyCollection<Guid> featureIds,
        IReadOnlyDictionary<string, string> properties,
        IReadOnlyCollection<FeatureDto> features,
        IReadOnlyCollection<string> supportedLanguages,
        IReadOnlyCollection<BotDetails> botDetails)
    {
        Id = id;
        UserName = userName;
        Token = token;
        FeatureIds = featureIds;
        Properties = properties;
        Features = features;
        SupportedLanguages = supportedLanguages;
        BotDetails = botDetails;
    }

    public IReadOnlyCollection<FeatureDto> SelectedFeatures => Features
        .Where(f => FeatureIds.Contains(f.Id))
        .ToArray();

    public static readonly StagesState Empty = new(
        null,
        string.Empty,
        string.Empty,
        Array.Empty<Guid>(),
        new Dictionary<string, string>(),
        Array.Empty<FeatureDto>(),
        Array.Empty<string>(),
        Array.Empty<BotDetails>());

    public StagesState Apply(IReadOnlyCollection<BotDetails> botDetails)
    {
        BotDetails = botDetails;

        return this;
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

    public StagesState Apply(SetSettingsFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        Properties = formModel.Properties.ToDictionary(v => v.Name, v => v.Value);
        SupportedLanguages = formModel.SupportedLanguages.ToArray();
        BotDetails = formModel.BotDetails
            .Select(b => new BotDetails(
                b.LanguageId,
                b.Name,
                b.ShortDescription,
                b.Description))
            .ToArray();
        
        return this;
    }
}