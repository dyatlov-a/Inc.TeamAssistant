using Inc.TeamAssistant.Primitives.FeatureProperties;
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
                        TeamProperties.NextReviewerTypeKey,
                        "Constructor_FormSectionSetSettingsNextReviewerStrategyFieldLabel",
                        GetValuesForNextReviewerType().ToArray())
                })
        };
    }
    
    private IEnumerable<SelectValue> GetValuesForNextReviewerType()
    {
        yield return new SelectValue(string.Empty, string.Empty);

        foreach (var item in Enum.GetValues<NextReviewerType>())
        {
            if (_storyType.TryGetValue(item, out var value))
                yield return new SelectValue(value, item.ToString());
        }
    }
}