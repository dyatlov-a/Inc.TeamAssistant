using FluentValidation;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModelValidator : AbstractValidator<SetSettingsFormModel>
{
    public SetSettingsFormModelValidator()
    {
        RuleFor(e => e.CalendarId)
            .NotEmpty();
        
        RuleFor(e => e.Properties)
            .NotEmpty();
        
        RuleForEach(e => e.Properties)
            .ChildRules(c =>
            {
                c.RuleFor(e => e.Title)
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
    }
}