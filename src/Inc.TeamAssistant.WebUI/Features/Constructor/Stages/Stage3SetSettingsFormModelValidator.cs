using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class Stage3SetSettingsFormModelValidator : AbstractValidator<Stage3SetSettingsFormModel>
{
    public Stage3SetSettingsFormModelValidator()
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
    }
}