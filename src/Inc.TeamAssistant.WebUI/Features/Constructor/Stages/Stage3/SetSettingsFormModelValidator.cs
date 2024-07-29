using FluentValidation;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModelValidator : AbstractValidator<SetSettingsFormModel>
{
    public SetSettingsFormModelValidator(IValidator<BotDetailsFormModel> botDetailsFormModelValidator)
    {
        RuleFor(e => e.Properties)
            .NotEmpty();
        
        RuleForEach(e => e.Properties)
            .ChildRules(c =>
            {
                c.RuleFor(e => e.Name)
                    .NotEmpty();

                c.RuleFor(e => e.Value)
                    .NotEmpty();
            });

        RuleFor(e => e.SupportedLanguages)
            .NotEmpty();
        
        RuleForEach(e => e.SupportedLanguages)
            .ChildRules(p =>
            {
                p.RuleFor(i => i)
                    .NotEmpty()
                    .Must(e => LanguageSettings.LanguageIds.Any(l => l.Value.Equals(
                        e,
                        StringComparison.InvariantCultureIgnoreCase)));
            });

        RuleFor(e => e.BotDetails)
            .NotEmpty();
        
        RuleForEach(e => e.BotDetails)
            .SetValidator(botDetailsFormModelValidator);
    }
}