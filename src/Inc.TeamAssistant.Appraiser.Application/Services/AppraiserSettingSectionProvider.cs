using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.FeatureProperties;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class AppraiserSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<StoryType, string> _storyType = new Dictionary<StoryType, string>
    {
        [StoryType.Fibonacci] = "Constructor_FormSectionSetSettingsFibonacciDescription",
        [StoryType.TShirt] = "Constructor_FormSectionSetSettingsTShirtDescription",
        [StoryType.PowerOfTwo] = "Constructor_FormSectionSetSettingsPowerOfTwoDescription"
    };

    public string FeatureName => "Appraiser";

    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return
        [
            new SettingSection(
                "Constructor_FormSectionSetSettingsAppraiserHeader",
                "Constructor_FormSectionSetSettingsAppraiserHelp",
                [
                    new(
                        AppraiserProperties.StoryTypeKey,
                        "Constructor_FormSectionSetSettingsStoryTypeFieldLabel",
                        GetValuesForStoryType().ToArray())
                ])
        ];
    }

    private IEnumerable<SelectValue> GetValuesForStoryType()
    {
        foreach (var item in Enum.GetValues<StoryType>())
            if (_storyType.TryGetValue(item, out var value))
                yield return new SelectValue(value, item.ToString());
    }
}