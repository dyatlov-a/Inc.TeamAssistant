using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Properties;

namespace Inc.TeamAssistant.Appraiser.Application.Services;

internal sealed class AppraiserSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<StoryType, string> _storyType = new Dictionary<StoryType, string>
    {
        [StoryType.Scrum] = "Constructor_FormSectionSetSettingsScrumDescription",
        [StoryType.Kanban] = "Constructor_FormSectionSetSettingsKanbanDescription"
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
                        "storyType",
                        "Constructor_FormSectionSetSettingsStoryTypeFieldLabel",
                        new[] { new SelectListItem(string.Empty, string.Empty) }
                            .Union(Enum.GetValues<StoryType>()
                                .Where(i => _storyType.ContainsKey(i))
                                .Select(i => new SelectListItem(_storyType[i], i.ToString())))
                            .ToArray())
                })
        };
    }
}