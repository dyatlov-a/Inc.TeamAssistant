using System.Globalization;
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

        var culture = new CultureInfo(languageId.Value);
        
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        
        validatorConfiguration.LanguageManager.Culture = culture;
        validatorConfiguration.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        validatorConfiguration.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        return validatorConfiguration;
    }
}