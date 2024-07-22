using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModelValidator : AbstractValidator<SetSettingsFormModel>
{
    public SetSettingsFormModelValidator()
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
    }
}