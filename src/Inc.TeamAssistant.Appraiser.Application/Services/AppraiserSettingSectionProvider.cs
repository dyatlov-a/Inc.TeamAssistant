using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Features.Properties;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class AppraiserSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<StoryType, string> _storyType = new Dictionary<StoryType, string>
    {
        [StoryType.Fibonacci] = "FormSectionSetSettingsFibonacciDescription",
        [StoryType.TShirt] = "FormSectionSetSettingsTShirtDescription",
        [StoryType.PowerOfTwo] = "FormSectionSetSettingsPowerOfTwoDescription"
    };

    public string FeatureName => "Appraiser";

    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return
        [
            new SettingSection(
                "FormSectionSetSettingsAppraiserHeader",
                "FormSectionSetSettingsAppraiserHelp",
                [
                    new(
                        AppraiserProperties.StoryTypeKey,
                        "FormSectionSetSettingsStoryTypeFieldLabel",
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