using FluentValidation;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Services;

public static class ValidatorConfigurationExtensions
{
    public static ValidatorConfiguration Configure(
        this ValidatorConfiguration validatorConfiguration,
        LanguageId languageId)
    {
        ArgumentNullException.ThrowIfNull(validatorConfiguration);
        ArgumentNullException.ThrowIfNull(languageId);
        
        validatorConfiguration.LanguageManager.Culture = new(languageId.Value);
        validatorConfiguration.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        validatorConfiguration.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        return validatorConfiguration;
    }
}