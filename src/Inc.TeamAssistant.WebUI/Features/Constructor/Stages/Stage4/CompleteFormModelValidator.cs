using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CompleteFormModelValidator : AbstractValidator<CompleteFormModel>
{
    public CompleteFormModelValidator()
    {
        RuleFor(e => e.CalendarId)
            .NotEmpty();
        
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