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
    public IReadOnlyCollection<string> PropertyKeys { get; private set; }
    public IReadOnlyDictionary<string, string> Properties { get; private set; }
    public IReadOnlyCollection<Feature> Features { get; private set; }

    public StagesState(
        Guid? id,
        string userName,
        string token,
        IReadOnlyCollection<Guid> featureIds,
        IReadOnlyCollection<string> propertyKeys,
        IReadOnlyDictionary<string, string> properties,
        IReadOnlyCollection<Feature> features)
    {
        Id = id;
        UserName = userName;
        Token = token;
        FeatureIds = featureIds;
        PropertyKeys = propertyKeys;
        Properties = properties;
        Features = features;
    }

    public static readonly StagesState Empty = new(
        null,
        string.Empty,
        string.Empty,
        Array.Empty<Guid>(),
        Array.Empty<string>(),
        new Dictionary<string, string>(),
        Array.Empty<Feature>());
    
    public sealed class Feature
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IReadOnlyCollection<string> Properties { get; set; } = Array.Empty<string>();
    }

    public void Apply(CheckBotFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        UserName = formModel.UserName;
        Token = formModel.Token;
    }

    public void Apply(SelectFeaturesFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        FeatureIds = formModel.FeatureIds.ToArray();
        PropertyKeys = Features
            .Where(f => formModel.FeatureIds.Contains(f.Id))
            .SelectMany(f => f.Properties)
            .ToArray();
        Properties = Properties
            .Where(p => PropertyKeys.Contains(p.Key, StringComparer.InvariantCultureIgnoreCase))
            .ToDictionary();
    }

    public void Apply(SetSettingsFormModel formModel)
    {
        ArgumentNullException.ThrowIfNull(formModel);
        
        Properties = formModel.Properties.ToDictionary(v => v.Name, v => v.Value!);
    }
}