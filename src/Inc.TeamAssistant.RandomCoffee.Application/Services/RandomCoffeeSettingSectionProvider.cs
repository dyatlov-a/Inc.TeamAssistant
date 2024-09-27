using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.Application.Services;

internal sealed class RandomCoffeeSettingSectionProvider : ISettingSectionProvider
{
    public string FeatureName => "RandomCoffee";
    
    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return
        [
            new SettingSection(
                new("Constructor_FormSectionSetSettingsRandomCoffeeHeader"),
                new("Constructor_FormSectionSetSettingsRandomCoffeeHelp"),
                [
                    new(
                        RandomCoffeeProperties.RoundIntervalKey,
                        new("Constructor_FormSectionSetSettingsRoundIntervalFieldLabel"),
                        GetValuesForRoundInterval().ToArray()),
                    new(
                        RandomCoffeeProperties.VotingIntervalKey,
                        new("Constructor_FormSectionSetSettingsVotingIntervalFieldLabel"),
                        GetValuesForVotingInterval().ToArray())
                ])
        ];
    }
    
    private IEnumerable<SelectValue> GetValuesForRoundInterval()
    {
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsRoundInterval1Description"), "7.00:00:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsRoundInterval2Description"), "14.00:00:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsRoundInterval3Description"), "21.00:00:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsRoundInterval4Description"), "28.00:00:00");
    }
    
    private IEnumerable<SelectValue> GetValuesForVotingInterval()
    {
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsVotingInterval1Description"), "02:00:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsVotingInterval2Description"), "04:00:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsVotingInterval3Description"), "1.00:00:00");
        yield return new SelectValue(new("Constructor_FormSectionSetSettingsVotingInterval4Description"), "2.00:00:00");
    }
}