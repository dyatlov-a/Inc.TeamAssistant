using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.FeatureProperties;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class AppraiserSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<StoryType, string> _storyType = new Dictionary<StoryType, string>
    {
        [StoryType.Fibonacci] = "Constructor_FormSectionSetSettingsScrumDescription",
        [StoryType.TShirt] = "Constructor_FormSectionSetSettingsKanbanDescription"
    };

    public string FeatureName => "Appraiser";

    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return new[]
        {
            new SettingSection(
                "Constructor_FormSectionSetSettingsAppraiserHeader",
                "Constructor_FormSectionSetSettingsAppraiserHelp",
                new SettingItem[]
                {
                    new(
                        TeamProperties.StoryTypeKey,
                        "Constructor_FormSectionSetSettingsStoryTypeFieldLabel",
                        GetValuesForStoryType().ToArray())
                })
        };
    }

    private IEnumerable<SelectValue> GetValuesForStoryType()
    {
        yield return new SelectValue(string.Empty, string.Empty);

        foreach (var item in Enum.GetValues<StoryType>())
        {
            if (_storyType.TryGetValue(item, out var value))
                yield return new SelectValue(value, item.ToString());
        }
    }
}