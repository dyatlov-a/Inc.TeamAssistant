using Inc.TeamAssistant.Primitives.Properties;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewerSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<NextReviewerType, string> _storyType = new Dictionary<NextReviewerType, string>
    {
        [NextReviewerType.RoundRobin] = "Constructor_FormSectionSetSettingsRoundRobinDescription",
        [NextReviewerType.Random] = "Constructor_FormSectionSetSettingsRandomDescription"
    };

    public string FeatureName => "Reviewer";

    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return new[]
        {
            new SettingSection(
                "Constructor_FormSectionSetSettingsReviewerHeader",
                "Constructor_FormSectionSetSettingsReviewerHelp",
                new SettingItem[]
                {
                    new(
                        "nextReviewerStrategy",
                        "Constructor_FormSectionSetSettingsNextReviewerStrategyFieldLabel",
                        new[] { new SelectListItem(string.Empty, string.Empty) }
                            .Union(Enum.GetValues<NextReviewerType>()
                                .Where(i => _storyType.ContainsKey(i))
                                .Select(i => new SelectListItem(_storyType[i], i.ToString())))
                            .ToArray())
                })
        };
    }
}