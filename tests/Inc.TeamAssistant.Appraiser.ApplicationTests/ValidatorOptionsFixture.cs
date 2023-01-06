using FluentValidation;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests;

internal sealed class ValidatorOptionsFixture
{
    public ValidatorOptionsFixture()
    {
        ValidatorOptions.Global.LanguageManager.Culture = new("en");
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }
}