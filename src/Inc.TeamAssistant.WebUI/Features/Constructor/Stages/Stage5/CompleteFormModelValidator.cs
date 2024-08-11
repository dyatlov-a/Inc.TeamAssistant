using FluentValidation;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage5;

public sealed class CompleteFormModelValidator : AbstractValidator<CompleteFormModel>
{
    public CompleteFormModelValidator(IValidator<BotDetailsFormModel> botDetailsFormModelValidator)
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
        
        RuleFor(e => e.BotDetails)
            .NotEmpty();
        
        RuleForEach(e => e.BotDetails)
            .SetValidator(botDetailsFormModelValidator);
    }
}