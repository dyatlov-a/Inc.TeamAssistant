using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewerSettingSectionProvider : ISettingSectionProvider
{
    private readonly IReadOnlyDictionary<NextReviewerType, MessageId> _storyType = new Dictionary<NextReviewerType, MessageId>
    {
        [NextReviewerType.RoundRobin] = new("Constructor_FormSectionSetSettingsRoundRobinDescription"),
        [NextReviewerType.Random] = new("Constructor_FormSectionSetSettingsRandomDescription")
    };

    public string FeatureName => "Reviewer";

    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return
        [
            new SettingSection(
                new("Constructor_FormSectionSetSettingsReviewerHeader"),
                new("Constructor_FormSectionSetSettingsReviewerHelp"),
                [
                    new(
                        ReviewerProperties.NextReviewerTypeKey,
                        new("Constructor_FormSectionSetSettingsNextReviewerStrategyFieldLabel"),
                        GetValuesForNextReviewerType().ToArray()),
                    new(
                        ReviewerProperties.WaitingNotificationIntervalKey,
                        new("Constructor_FormSectionSetSettingsWaitingNotificationIntervalFieldLabel"),
                        GetValuesForNotificationInterval().ToArray()),
                    new(
                        ReviewerProperties.InProgressNotificationIntervalKey,
                        new("Constructor_FormSectionSetSettingsInProgressNotificationIntervalFieldLabel"),
                        GetValuesForNotificationInterval().ToArray()),
                    new(
                        ReviewerProperties.AcceptWithCommentsKey,
                        new("Constructor_FormSectionSetSettingsAcceptWithCommentsFieldLabel"),
                        GetValuesForAcceptWithComments().ToArray())
                ])
        ];
    }
    
    private IEnumerable<SelectValue> GetValuesForNextReviewerType()
    {
        foreach (var item in Enum.GetValues<NextReviewerType>())
            if (_storyType.TryGetValue(item, out var value))
                yield return new SelectValue(value, item.ToString());
    }
    
    private IEnumerable<SelectValue> GetValuesForNotificationInterval()
    {
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsNotificationInterval1Description"), "00:30:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsNotificationInterval2Description"), "01:00:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsNotificationInterval3Description"), "01:30:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsNotificationInterval4Description"), "02:00:00");
    }
    
    private IEnumerable<SelectValue> GetValuesForAcceptWithComments()
    {
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsAcceptWithCommentsTrueDescription"), "true");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsAcceptWithCommentsFalseDescription"), "false");
    }
}