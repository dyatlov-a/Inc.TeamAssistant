using Inc.TeamAssistant.Primitives.Features.Properties;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewerSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<NextReviewerType, string> _nextReviewerType = new Dictionary<NextReviewerType, string>
    {
        [NextReviewerType.RoundRobin] = "FormSectionSetSettingsRoundRobinDescription",
        [NextReviewerType.RoundRobinForTeam] = "FormSectionSetSettingsRoundRobinForTeamDescription",
        [NextReviewerType.Random] = "FormSectionSetSettingsRandomDescription"
    };

    public string FeatureName => "Reviewer";

    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return
        [
            new SettingSection(
                "FormSectionSetSettingsReviewerHeader",
                "FormSectionSetSettingsReviewerHelp",
                [
                    new(
                        ReviewerProperties.NextReviewerTypeKey,
                        "FormSectionSetSettingsNextReviewerStrategyFieldLabel",
                        GetValuesForNextReviewerType().ToArray()),
                    new(
                        ReviewerProperties.WaitingNotificationIntervalKey,
                        "FormSectionSetSettingsWaitingNotificationIntervalFieldLabel",
                        GetValuesForNotificationInterval().ToArray()),
                    new(
                        ReviewerProperties.InProgressNotificationIntervalKey,
                        "FormSectionSetSettingsInProgressNotificationIntervalFieldLabel",
                        GetValuesForNotificationInterval().ToArray())
                ])
        ];
    }
    
    private IEnumerable<SelectValue> GetValuesForNextReviewerType()
    {
        var selectableValues = Enum.GetValues<NextReviewerType>()
            .Where(t => t != NextReviewerType.Target)
            .ToArray();
        
        foreach (var item in selectableValues)
            if (_nextReviewerType.TryGetValue(item, out var value))
                yield return new SelectValue(value, item.ToString());
    }
    
    private IEnumerable<SelectValue> GetValuesForNotificationInterval()
    {
        yield return new SelectValue("FormSectionSetSettingsNotificationInterval1Description", "00:30:00");
        yield return new SelectValue("FormSectionSetSettingsNotificationInterval2Description", "01:00:00");
        yield return new SelectValue("FormSectionSetSettingsNotificationInterval3Description", "01:30:00");
        yield return new SelectValue("FormSectionSetSettingsNotificationInterval4Description", "02:00:00");
    }
}