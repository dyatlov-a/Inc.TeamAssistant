using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

internal sealed class BotDetailsFormModelValidator : AbstractValidator<BotDetailsFormModel>
{
    public BotDetailsFormModelValidator(IValidator<BotDetailsItemFormModel> botDetailsFormModelValidator)
    {
        ArgumentNullException.ThrowIfNull(botDetailsFormModelValidator);
        
        RuleFor(e => e.BotDetails)
            .NotEmpty();
        
        RuleForEach(e => e.BotDetails)
            .SetValidator(botDetailsFormModelValidator);
    }
}