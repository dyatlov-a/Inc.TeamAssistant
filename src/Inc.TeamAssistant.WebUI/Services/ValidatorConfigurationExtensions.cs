using System.Globalization;
using FluentValidation;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Services;

public static class ValidatorConfigurationExtensions
{
    public static async Task<CultureInfo> SetCulture(
        this ValidatorConfiguration validatorConfiguration,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(validatorConfiguration);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var renderContext = serviceProvider.GetRequiredService<IRenderContext>();
        var resourcesManager = serviceProvider.GetRequiredService<ResourcesManager>();
        
        var languageContext = renderContext.GetLanguageContext();
        var culture = new CultureInfo(languageContext.CurrentLanguage.Value);
        
        validatorConfiguration.LanguageManager.Culture = culture;
        validatorConfiguration.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        validatorConfiguration.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        await resourcesManager.Initialize();

        return culture;
    }
}