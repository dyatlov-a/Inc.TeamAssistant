namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public static class BotSettingsFactory
{
    public sealed record SettingSection(string Header, string Help, IReadOnlyCollection<SettingItem> SettingItems);

    public sealed record SettingItem(string PropertyName, string Label, IReadOnlyCollection<SelectListItem> Values)
    {
        public string GetValueAsText(string value)
        {
            return Values.SingleOrDefault(s => s.Value == value)?.Text ?? string.Empty;
        }
    }

    public sealed record SelectListItem(string Text, string Value);

    public static IReadOnlyDictionary<string, SettingSection> Create(IReadOnlyDictionary<string, string> resources)
    {
        ArgumentNullException.ThrowIfNull(resources);
        
        return new Dictionary<string, SettingSection>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["storyType"] = new(
                resources[Messages.Constructor_FormSectionSetSettingsAppraiserHeader],
                resources[Messages.Constructor_FormSectionSetSettingsAppraiserHelp],
                new SettingItem[]
                {
                    new(
                        "story-type",
                        resources[Messages.Constructor_FormSectionSetSettingsStoryTypeFieldLabel],
                        new SelectListItem[]
                        {
                            new(resources[Messages.Constructor_FormSectionSetSettingsScrumDescription], "Scrum"),
                            new(resources[Messages.Constructor_FormSectionSetSettingsKanbanDescription], "Kanban")
                        })
                }),
            ["nextReviewerStrategy"] = new(
                resources[Messages.Constructor_FormSectionSetSettingsReviewerHeader],
                resources[Messages.Constructor_FormSectionSetSettingsReviewerHelp],
                new SettingItem[]
                {
                    new(
                        "next-reviewer-strategy",
                        resources[Messages.Constructor_FormSectionSetSettingsNextReviewerStrategyFieldLabel],
                        new SelectListItem[]
                        {
                            new(resources[Messages.Constructor_FormSectionSetSettingsRoundRobinDescription], "RoundRobin"),
                            new(resources[Messages.Constructor_FormSectionSetSettingsRandomDescription], "Random")
                        })
                })
        };
    }
}