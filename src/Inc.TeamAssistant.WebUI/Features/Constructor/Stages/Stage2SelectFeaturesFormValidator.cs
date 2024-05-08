using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class Stage2SelectFeaturesFormValidator : AbstractValidator<Stage2SelectFeaturesFormModel>
{
    public Stage2SelectFeaturesFormValidator()
    {
        RuleFor(e => e.FeatureIds)
            .NotEmpty();

        RuleForEach(e => e.FeatureIds)
            .NotEmpty();
    }
}