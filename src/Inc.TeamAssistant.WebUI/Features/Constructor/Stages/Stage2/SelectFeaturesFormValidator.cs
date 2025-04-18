using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage2;

internal sealed class SelectFeaturesFormValidator : AbstractValidator<SelectFeaturesFormModel>
{
    public SelectFeaturesFormValidator()
    {
        RuleFor(e => e.FeatureIds)
            .NotEmpty();

        RuleForEach(e => e.FeatureIds)
            .NotEmpty();
    }
}