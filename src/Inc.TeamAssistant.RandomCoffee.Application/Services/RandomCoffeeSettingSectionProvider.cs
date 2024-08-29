using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.Application.Services;

internal sealed class RandomCoffeeSettingSectionProvider : ISettingSectionProvider
{
    public string FeatureName => "RandomCoffee";
    
    public IReadOnlyCollection<SettingSection> GetSections()
    {
        return new[]
        {
            new SettingSection(
                "Constructor_FormSectionSetSettingsRandomCoffeeHeader",
                "Constructor_FormSectionSetSettingsRandomCoffeeHelp",
                new SettingItem[]
                {
                    new(
                        BotProperties.RoundIntervalKey,
                        "Constructor_FormSectionSetSettingsRoundIntervalFieldLabel",
                        GetValuesForRoundInterval().ToArray()),
                    new(
                        BotProperties.VotingIntervalKey,
                        "Constructor_FormSectionSetSettingsVotingIntervalFieldLabel",
                        GetValuesForVotingInterval().ToArray())
                })
        };
    }
    
    private IEnumerable<SelectValue> GetValuesForRoundInterval()
    {
        yield return new SelectValue("Constructor_FormSectionSetSettingsRoundInterval1Description", "7.00:00:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsRoundInterval2Description", "14.00:00:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsRoundInterval3Description", "21.00:00:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsRoundInterval4Description", "28.00:00:00");
    }
    
    private IEnumerable<SelectValue> GetValuesForVotingInterval()
    {
        yield return new SelectValue("Constructor_FormSectionSetSettingsVotingInterval1Description", "02:00:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsVotingInterval2Description", "04:00:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsVotingInterval3Description", "1.00:00:00");
        yield return new SelectValue("Constructor_FormSectionSetSettingsVotingInterval4Description", "2.00:00:00");
    }
}