using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class Stage1CheckBotFormModelValidator : AbstractValidator<Stage1CheckBotFormModel>
{
    public Stage1CheckBotFormModelValidator()
    {
        RuleFor(e => e.Token)
            .NotEmpty()
            .Must((c, p) => c.HasAccess)
            .WithMessage("The token is invalid.");

        RuleFor(e => e.UserName)
            .NotEmpty();
    }
}