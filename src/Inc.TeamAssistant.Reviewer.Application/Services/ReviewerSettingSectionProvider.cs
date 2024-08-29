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
                        GetValuesForNextReviewerType().ToArray()),
                    new(
                        TeamProperties.WaitingNotificationIntervalKey,
                        "Constructor_FormSectionSetSettingsWaitingNotificationIntervalFieldLabel",
                        GetValuesForNotificationInterval().ToArray()),
                    new(
                        TeamProperties.InProgressNotificationIntervalKey,
                        "Constructor_FormSectionSetSettingsInProgressNotificationIntervalFieldLabel",
                        GetValuesForNotificationInterval().ToArray())
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
    
    private IEnumerable<SelectValue> GetValuesForNotificationInterval()
    {
        yield return new SelectValue(string.Empty, string.Empty);

        yield return new SelectValue("Constructor_FormSectionSetSettingsNotificationInterval1Description", "00:30:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsNotificationInterval2Description", "01:00:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsNotificationInterval3Description", "01:30:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsNotificationInterval4Description", "02:00:00");
    }
}