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
                "FormSectionSetSettingsRandomCoffeeHeader",
                "FormSectionSetSettingsRandomCoffeeHelp",
                [
                    new(
                        RandomCoffeeProperties.RoundIntervalKey,
                        "FormSectionSetSettingsRoundIntervalFieldLabel",
                        GetValuesForRoundInterval().ToArray()),
                    new(
                        RandomCoffeeProperties.VotingIntervalKey,
                        "FormSectionSetSettingsVotingIntervalFieldLabel",
                        GetValuesForVotingInterval().ToArray())
                ])
        ];
    }
    
    private IEnumerable<SelectValue> GetValuesForRoundInterval()
    {
        yield return new SelectValue("FormSectionSetSettingsRoundInterval1Description", "7.00:00:00");
        yield return new SelectValue("FormSectionSetSettingsRoundInterval2Description", "14.00:00:00");
        yield return new SelectValue("FormSectionSetSettingsRoundInterval3Description", "21.00:00:00");
        yield return new SelectValue("FormSectionSetSettingsRoundInterval4Description", "28.00:00:00");
    }
    
    private IEnumerable<SelectValue> GetValuesForVotingInterval()
    {
        yield return new SelectValue("FormSectionSetSettingsVotingInterval1Description", "02:00:00");
        yield return new SelectValue("FormSectionSetSettingsVotingInterval2Description", "04:00:00");
        yield return new SelectValue("FormSectionSetSettingsVotingInterval3Description", "1.00:00:00");
        yield return new SelectValue("FormSectionSetSettingsVotingInterval4Description", "2.00:00:00");
    }
}