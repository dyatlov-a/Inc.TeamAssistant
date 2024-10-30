using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

namespace Inc.TeamAssistant.Stories.Features.Constructor;

internal static class StagesStateFactory
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties =
        new Dictionary<string, IReadOnlyCollection<SettingSection>>
        {
            ["Appraiser"] =
            [
                new SettingSection(
                    HeaderMessageId: "FormSectionSetSettingsAppraiserHeader",
                    HelpMessageId: "FormSectionSetSettingsAppraiserHelp",
                    SettingItems:
                    [
                        new SettingItem(
                            PropertyName: "storyType",
                            LabelMessageId: "FormSectionSetSettingsStoryTypeFieldLabel",
                            Values:
                            [
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsFibonacciDescription",
                                    Value: "Fibonacci"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsTShirtDescription",
                                    Value: "TShirt"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsPowerOfTwoDescription",
                                    Value: "PowerOfTwo")
                            ])
                    ])
            ],
            ["Reviewer"] =
            [
                new SettingSection(
                    HeaderMessageId: "FormSectionSetSettingsReviewerHeader",
                    HelpMessageId: "FormSectionSetSettingsReviewerHelp",
                    SettingItems:
                    [
                        new SettingItem(
                            PropertyName: "nextReviewerStrategy",
                            LabelMessageId: "FormSectionSetSettingsNextReviewerStrategyFieldLabel",
                            Values:
                            [
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsRoundRobinDescription",
                                    Value: "RoundRobin"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsRandomDescription",
                                    Value: "Random")
                            ]),
                        new SettingItem(
                            PropertyName: "waitingNotificationInterval",
                            LabelMessageId: "FormSectionSetSettingsWaitingNotificationIntervalFieldLabel",
                            Values:
                            [
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval1Description",
                                    Value: "00:30:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval2Description",
                                    Value: "01:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval3Description",
                                    Value: "01:30:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval4Description",
                                    Value: "02:00:00")
                            ]),
                        new SettingItem(
                            PropertyName: "inProgressNotificationInterval",
                            LabelMessageId: "FormSectionSetSettingsInProgressNotificationIntervalFieldLabel",
                            Values:
                            [
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval1Description",
                                    Value: "00:30:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval2Description",
                                    Value: "01:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval3Description",
                                    Value: "01:30:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsNotificationInterval4Description",
                                    Value: "02:00:00")
                            ]),
                        new SettingItem(
                            PropertyName: "acceptWithComments",
                            LabelMessageId: "FormSectionSetSettingsAcceptWithCommentsFieldLabel",
                            Values:
                            [
                                new SelectValue(
                                    MessageId: "BooleanTrueText",
                                    Value: "true"),
                                new SelectValue(
                                    MessageId: "BooleanFalseText",
                                    Value: "false")
                            ])
                    ])
            ],
            ["RandomCoffee"] =
            [
                new SettingSection(
                    HeaderMessageId: "FormSectionSetSettingsRandomCoffeeHeader",
                    HelpMessageId: "FormSectionSetSettingsRandomCoffeeHelp",
                    SettingItems:
                    [
                        new SettingItem(
                            PropertyName: "roundInterval",
                            LabelMessageId: "FormSectionSetSettingsRoundIntervalFieldLabel",
                            Values:
                            [
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsRoundInterval1Description",
                                    Value: "7.00:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsRoundInterval2Description",
                                    Value: "14.00:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsRoundInterval3Description",
                                    Value: "21.00:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsRoundInterval4Description",
                                    Value: "28.00:00:00")
                            ]),
                        new SettingItem(
                            PropertyName: "votingInterval",
                            LabelMessageId: "FormSectionSetSettingsVotingIntervalFieldLabel",
                            Values:
                            [
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsVotingInterval1Description",
                                    Value: "02:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsVotingInterval2Description",
                                    Value: "04:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsVotingInterval3Description",
                                    Value: "1.00:00:00"),
                                new SelectValue(
                                    MessageId: "FormSectionSetSettingsVotingInterval4Description",
                                    Value: "2.00:00:00")
                            ])
                    ])
            ]
        };
    
    public static StagesState Create(IReadOnlyCollection<string>? features = null)
    {
        var availableProperties = features is null
            ? AvailableProperties
            : AvailableProperties
                .Where(p => features.Contains(p.Key, StringComparer.InvariantCultureIgnoreCase))
                .ToDictionary();
        
        return new(
            id: Guid.NewGuid(),
            calendarId: null,
            userName: "inc_teamassistant_bot",
            token: Guid.NewGuid().ToString(),
            supportedLanguages: ["en", "ru"],
            featureIds:
            [
                Guid.Parse("dd70b5cb-dadf-4290-bdef-812dd70678c1"),
                Guid.Parse("00d7657c-8d52-4566-8ca4-778198ef450f"),
                Guid.Parse("01142088-5dbd-4438-b594-c53de1427582"),
                Guid.Parse("7fce1684-cf79-4169-9ab4-e7eed7a2a418")
            ],
            properties: new Dictionary<string, string>
            {
                ["storyType"] = "Fibonacci",
                ["nextReviewerStrategy"] = "RoundRobin",
                ["waitingNotificationInterval"] = "00:30:00",
                ["inProgressNotificationInterval"] = "01:00:00",
                ["acceptWithComments"] = "true",
                ["roundInterval"] = "14.00:00:00",
                ["votingInterval"] = "1.00:00:00"
            },
            availableFeatures:
            [
                new FeatureDto(
                    Id: Guid.Parse("dd70b5cb-dadf-4290-bdef-812dd70678c1"),
                    Name: "Appraiser",
                    Properties: ["storyType"]),
                new FeatureDto(
                    Id: Guid.Parse("00d7657c-8d52-4566-8ca4-778198ef450f"),
                    Name: "Reviewer",
                    Properties: [
                        "nextReviewerStrategy",
                        "waitingNotificationInterval",
                        "inProgressNotificationInterval",
                        "acceptWithComments"
                    ]),
                new FeatureDto(
                    Id: Guid.Parse("01142088-5dbd-4438-b594-c53de1427582"),
                    Name: "RandomCoffee",
                    Properties: ["roundInterval", "votingInterval"]),
                new FeatureDto(
                    Id: Guid.Parse("7fce1684-cf79-4169-9ab4-e7eed7a2a418"),
                    Name: "CheckIn",
                    Properties: Array.Empty<string>())
            ],
            availableProperties: availableProperties);
    }
}