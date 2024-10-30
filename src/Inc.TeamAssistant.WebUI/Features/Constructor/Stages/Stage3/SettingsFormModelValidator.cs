using FluentValidation;
using Inc.TeamAssistant.Primitives.Languages;
using Microsoft.Extensions.Localization;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

internal sealed class SettingsFormModelValidator : AbstractValidator<SettingsFormModel>
{
    public SettingsFormModelValidator(IStringLocalizer<ConstructorResources> localizer)
    {
        ArgumentNullException.ThrowIfNull(localizer);
        
        RuleFor(e => e.CalendarId)
            .NotEmpty()
            .WithMessage(localizer["RequiredCalendar"]);
        
        RuleFor(e => e.Properties)
            .NotNull();
        
        RuleForEach(e => e.Properties)
            .ChildRules(c =>
            {
                c.RuleFor(e => e.Key)
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