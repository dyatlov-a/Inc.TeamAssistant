using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

public sealed class FeaturesFactory
{
    private readonly ResourcesManager _resources;

    public FeaturesFactory(ResourcesManager resources)
    {
        _resources = resources ?? throw new ArgumentNullException(nameof(resources));
    }

    public string CreateName(string featureName) => Create(featureName, "Name");
    
    public string CreateDescription(string featureName) => Create(featureName, "Description");

    private string Create(string featureName, string propertyName)
    {
        var key = new MessageId($"Constructor_Feature{featureName}{propertyName}");

        return _resources[key];
    }
}