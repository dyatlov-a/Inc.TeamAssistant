using FluentValidation;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModelValidator : AbstractValidator<SetSettingsFormModel>
{
    public SetSettingsFormModelValidator(ResourcesManager resources)
    {
        ArgumentNullException.ThrowIfNull(resources);
        
        RuleFor(e => e.CalendarId)
            .NotEmpty()
            .WithMessage(resources[Messages.Constructor_RequiredCalendar]);
        
        RuleFor(e => e.Properties)
            .NotNull();
        
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