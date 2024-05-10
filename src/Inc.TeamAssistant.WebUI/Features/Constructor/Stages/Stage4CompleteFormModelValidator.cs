using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class Stage4CompleteFormModelValidator : AbstractValidator<Stage4CompleteFormModel>
{
    public Stage4CompleteFormModelValidator()
    {
        RuleFor(e => e.UserName)
            .NotEmpty();

        RuleFor(e => e.Token)
            .NotEmpty();

        RuleFor(e => e.FeatureIds)
            .NotEmpty();

        RuleForEach(e => e.FeatureIds)
            .NotEmpty();

        RuleForEach(e => e.Properties)
            .ChildRules(c =>
            {
                c.RuleFor(e => e.Key)
                    .NotEmpty();

                c.RuleFor(e => e.Value)
                    .NotEmpty();
            });
    }
}