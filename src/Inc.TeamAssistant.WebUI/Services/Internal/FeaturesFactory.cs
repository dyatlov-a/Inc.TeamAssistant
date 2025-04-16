using Inc.TeamAssistant.WebUI.Features.Constructor;
using Microsoft.Extensions.Localization;

namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal sealed class FeaturesFactory
{
    private readonly IStringLocalizer<ConstructorResources> _localizer;

    public FeaturesFactory(IStringLocalizer<ConstructorResources> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public string CreateName(string featureName) => Create(featureName, "Name");
    
    public string CreateDescription(string featureName) => Create(featureName, "Description");

    private string Create(string featureName, string propertyName)
    {
        return _localizer[$"Feature{featureName}{propertyName}"];
    }
}