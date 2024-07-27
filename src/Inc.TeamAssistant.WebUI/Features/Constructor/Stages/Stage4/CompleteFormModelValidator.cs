using FluentValidation;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CompleteFormModelValidator : AbstractValidator<CompleteFormModel>
{
    public CompleteFormModelValidator(BotDetailsFormModelValidator botDetailsFormModelValidator)
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
        
        RuleForEach(e => e.BotDetails)
            .SetValidator(botDetailsFormModelValidator);
    }
}