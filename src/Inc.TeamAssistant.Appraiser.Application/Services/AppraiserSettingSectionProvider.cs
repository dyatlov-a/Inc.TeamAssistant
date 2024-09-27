using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class AppraiserSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<StoryType, MessageId> _storyType = new Dictionary<StoryType, MessageId>
    {
        [StoryType.Fibonacci] = new("Constructor_FormSectionSetSettingsFibonacciDescription"),
        [StoryType.TShirt] = new("Constructor_FormSectionSetSettingsTShirtDescription"),
        [StoryType.PowerOfTwo] = new("Constructor_FormSectionSetSettingsPowerOfTwoDescription")
    };

    public string FeatureName => "Appraiser";

    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return
        [
            new SettingSection(
                new("Constructor_FormSectionSetSettingsAppraiserHeader"),
                new("Constructor_FormSectionSetSettingsAppraiserHelp"),
                [
                    new(
                        AppraiserProperties.StoryTypeKey,
                        new("Constructor_FormSectionSetSettingsStoryTypeFieldLabel"),
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